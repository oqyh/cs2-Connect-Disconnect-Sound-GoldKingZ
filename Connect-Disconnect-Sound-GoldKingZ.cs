using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Utils;
using CnD_Sound.Config;

namespace CnD_Sound;

public class MainPlugin : BasePlugin
{
    public override string ModuleName => "Connect Disconnect Sound (Continent , Country , City , Message , Sounds , Logs , Discord)";
    public override string ModuleVersion => "1.1.1";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    public static MainPlugin Instance { get; set; } = new();
    public Globals g_Main = new();
    private readonly PlayerChat _PlayerChat = new();
    
    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory);

        _ = Task.Run(async () => 
        {
            try
            {
                await Helper.DownloadMissingFiles();
                
                var geoUpdated = await Helper.CheckAndUpdateGeoAsync();
                if (geoUpdated)
                {
                    Helper.DebugMessage("GeoLite2-City.mmdb Updated Successfully!");
                }

                await Server.NextFrameAsync(() => CustomHooks.StartHook());
            }
            catch (Exception ex)
            {
                Helper.DebugMessage($"DownloadMissingFiles/geoUpdated failed: {ex}");
            }
        });

        
        RegisterEventHandler<EventPlayerConnectFull>(OnEventPlayerConnectFull);
        RegisterEventHandler<EventPlayerDeath>(OnEventPlayerDeath, HookMode.Pre);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect, HookMode.Pre);
        RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);

        RegisterListener<Listeners.OnClientAuthorized>(OnClientAuthorized);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        
        AddCommandListener("say", OnPlayerChat, HookMode.Post);
        AddCommandListener("say_team", OnPlayerChatTeam, HookMode.Post);

        if(hotReload)
        {
            Helper.LoadJson();

            if (Configs.GetConfigData().MySql_Enable)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {                        
                        await MySqlDataManager.CreateTableIfNotExistsAsync();
                        await MySqlDataManager.DeleteOldPlayersAsync();
                    }
                    catch (Exception ex)
                    {
                        Helper.DebugMessage($"hotReload error: {ex.Message}");
                    }
                });
            }
        }
    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        try
        {
            string filePath = Path.Combine(ModuleDirectory, "config/ServerPrecacheResources.txt");
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (line.TrimStart().StartsWith("//"))continue;
                manifest.AddResource(line);
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage(ex.Message);
        }
    }


    public void OnMapStart(string Map)
    {
        Helper.LoadJson();

        g_Main.ServerPublicIpAdress = ConVar.Find("ip")?.StringValue!;
        g_Main.ServerPort = ConVar.Find("hostport")?.GetPrimitiveValue<int>().ToString()!;

        if(Configs.GetConfigData().Log_Locally_AutoDeleteLogsMoreThanXdaysOld > 0)
        {
            string Fpath = Path.Combine(ModuleDirectory,"logs");
            Helper.DeleteOldFiles(Fpath, "*" + ".txt", TimeSpan.FromDays(Configs.GetConfigData().Log_Locally_AutoDeleteLogsMoreThanXdaysOld));
        }

        _ = Task.Run(async () => 
        {
            if(string.IsNullOrEmpty(g_Main.ServerPublicIpAdress))
            {
                string ip = await Helper.GetPublicIp();
                if(!string.IsNullOrEmpty(ip))
                {
                    g_Main.ServerPublicIpAdress = ip;
                }
            }

            if (Configs.GetConfigData().MySql_Enable)
            {
                await MySqlDataManager.CreateTableIfNotExistsAsync();
            }
        });
    }

    private void OnClientAuthorized(int playerSlot, SteamID steamId)
    {
        if (!Configs.GetConfigData().EarlyConnection) return;

        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if (!player.IsValid()) return;

        _ = HandlePlayerConnectionsAsync(player, false, "");
    }

    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;

        var player = @event.Userid;
        if (!player.IsValid())return HookResult.Continue;

        if(Configs.GetConfigData().DisableLoopConnections)
        {
            if (g_Main.OnLoop.ContainsKey(player))
            {
                g_Main.OnLoop.Remove(player);
            }
        }

        if (Configs.GetConfigData().EarlyConnection)return HookResult.Continue;

        _ = HandlePlayerConnectionsAsync(player, false, "");

        return HookResult.Continue;
    }

    private HookResult OnEventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        var victim = @event.Userid;
        if(!victim.IsValid(false))return HookResult.Continue;

        if (Configs.GetConfigData().RemoveDefaultDisconnect == 2)
        {
            if(g_Main.Player_Data.ContainsKey(victim) && g_Main.Player_Data[victim].Remove_Icon)
            {
                info.DontBroadcast = true;
            }
        }
        
        return HookResult.Continue;
    }

    private async Task HandlePlayerConnectionsAsync(CCSPlayerController Getplayer, bool Disconnect, string reason)
    {
        try
        {
            if(Configs.GetConfigData().DisableServerHibernate)
            {
                Server.ExecuteCommand("sv_hibernate_when_empty false");
            }

            var player = Getplayer;
            if (!player.IsValid()) return;

            var playername = player.PlayerName;
            var playersteamid = player.SteamID;
            var playersteamId2 = player.AuthorizedSteamID?.SteamId2 ?? Localizer["invalid.steamid"];
            var playersteamId3 = player.AuthorizedSteamID?.SteamId3 ?? Localizer["invalid.steamid"];
            var playersteamId32 = player.AuthorizedSteamID?.SteamId32.ToString() ?? Localizer["invalid.steamid"];
            var playersteamId64 = player.AuthorizedSteamID?.SteamId64.ToString() ?? Localizer["invalid.steamid"];
            var playerip = player.IpAddress?.Split(':')[0] ?? Localizer["InValidIpAddress"];

            if(!Disconnect)
            {
                await Helper.LoadPlayerData(player);
            }

            var geoInfo = await Helper.GetGeoInfoAsync(playerip);
            
            await Server.NextFrameAsync(async () =>
            {
                if (!player.IsValid() || !g_Main.Player_Data.ContainsKey(player))return;

                var (ConnectionSettingsMessage, ConnectionSettingsSound, ConnectionSettingsSoundVolume) = Helper.GetPlayerConnectionSettings(player, Disconnect?"DISCONNECT":"CONNECT");
                string formatted = "";

                if (!string.IsNullOrEmpty(ConnectionSettingsMessage))
                {
                    formatted = ConnectionSettingsMessage.ReplaceMessages(
                        DateTime.Now.ToString(Configs.GetConfigData().DateFormat),
                        DateTime.Now.ToString(Configs.GetConfigData().TimeFormat),
                        playername,
                        playersteamId2,
                        playersteamId3,
                        playersteamId32,
                        playersteamId64,
                        playerip,
                        geoInfo.Continent,
                        geoInfo.Country,
                        geoInfo.CountryCode,
                        geoInfo.City,
                        Disconnect?reason:""
                    );
                }

                foreach (var allplayers in Helper.GetPlayersController())
                {
                    if (!allplayers.IsValid() || !g_Main.Player_Data.ContainsKey(allplayers))continue;

                    if (!string.IsNullOrEmpty(formatted) && (g_Main.Player_Data[allplayers].Toggle_Messages == 1 || g_Main.Player_Data[allplayers].Toggle_Messages == -1))
                    {
                        formatted = formatted.ReplaceColorTags();
                        Helper.AdvancedPlayerPrintToChat(allplayers, formatted);
                    }
                    
                    if (ConnectionSettingsSound.Count > 0 && (g_Main.Player_Data[allplayers].Toggle_Sounds == 1 || g_Main.Player_Data[allplayers].Toggle_Sounds == -1))
                    {
                        string nextSound = ConnectionSettingsSound.GetNextSound(pickRandom: Configs.GetConfigData().PickRandomSounds);

                        if(nextSound.StartsWith("sounds/"))
                        {
                            allplayers.ExecuteClientCommand($"play {nextSound}");
                        }else
                        {
                            float SoundVolume = ConnectionSettingsSoundVolume.ToPercentageFloat();
                            RecipientFilter filter = [allplayers];
                            allplayers.EmitSound(nextSound, filter, (float)SoundVolume);
                        }
                    }
                }

                if (Configs.GetConfigData().Log_Locally_Enable)
                {
                    var logPath = Path.Combine(ModuleDirectory, "logs");
                    Directory.CreateDirectory(logPath);
                    
                    var fileName = DateTime.Now.ToString(Configs.GetConfigData().Log_Locally_DateFormat) + ".txt";
                    var fullPath = Path.Combine(logPath, fileName);
                    string Format = Disconnect?Configs.GetConfigData().Log_Locally_Disconnect_Format:Configs.GetConfigData().Log_Locally_Connect_Format;
                    var logMessage = Format?.ReplaceMessages(
                        DateTime.Now.ToString(Configs.GetConfigData().Log_Locally_DateFormat),
                        DateTime.Now.ToString(Configs.GetConfigData().Log_Locally_TimeFormat),
                        playername,
                        playersteamId2,
                        playersteamId3,
                        playersteamId32,
                        playersteamId64,
                        playerip,
                        geoInfo.Continent,
                        geoInfo.Country,
                        geoInfo.CountryCode,
                        geoInfo.City,
                        Disconnect?reason:""
                    );

                    if (!string.IsNullOrEmpty(logMessage))
                    {
                        await File.AppendAllTextAsync(fullPath, logMessage + Environment.NewLine);
                    }
                }
            });

            if (Disconnect?!string.IsNullOrEmpty(Configs.GetConfigData().Discord_Disconnect_WebHook):!string.IsNullOrEmpty(Configs.GetConfigData().Discord_Connect_WebHook))
            {
                string Format = Disconnect?Configs.GetConfigData().Discord_Disconnect_Format:Configs.GetConfigData().Discord_Connect_Format;
                var discordMessage = Format?.ReplaceMessages(
                    DateTime.Now.ToString(Configs.GetConfigData().Discord_DateFormat),
                    DateTime.Now.ToString(Configs.GetConfigData().Discord_TimeFormat),
                    playername,
                    playersteamId2,
                    playersteamId3,
                    playersteamId32,
                    playersteamId64,
                    playerip,
                    geoInfo.Continent,
                    geoInfo.Country,
                    geoInfo.CountryCode,
                    geoInfo.City,
                    Disconnect?reason:""
                );

                if (!string.IsNullOrEmpty(discordMessage))
                {
                    await Helper.SendToDiscordAsync(
                        Disconnect?Configs.GetConfigData().Discord_Disconnect_Style:Configs.GetConfigData().Discord_Connect_Style,
                        Disconnect?Configs.GetConfigData().Discord_Disconnect_WebHook:Configs.GetConfigData().Discord_Connect_WebHook,
                        discordMessage,
                        playersteamId64,
                        playername,
                        $"{g_Main.ServerPublicIpAdress}:{g_Main.ServerPort}",
                        Disconnect
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"HandlePlayerConnectionsAsync error: {ex.Message}");
        }
    }

    private HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info)
    {
        if (!player.IsValid())return HookResult.Continue;

        _PlayerChat.OnPlayerChat(player, info, false);

        return HookResult.Continue;
    }
    private HookResult OnPlayerChatTeam(CCSPlayerController? player, CommandInfo info)
    {
        if (!player.IsValid())return HookResult.Continue;

        _PlayerChat.OnPlayerChat(player, info, true);

        return HookResult.Continue;
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;

        if (Configs.GetConfigData().RemoveDefaultDisconnect == 1 || Configs.GetConfigData().RemoveDefaultDisconnect == 2)
        {
            info.DontBroadcast = true;
        }

        var player = @event.Userid;
        var reasonInt = @event.Reason;
        var reason = Helper.GetDisconnectReason(reasonInt);

        if (!player.IsValid())return HookResult.Continue;

        if (Configs.GetConfigData().DisableLoopConnections && reasonInt == 55)
        {
            if (!g_Main.OnLoop.ContainsKey(player))
            {
                g_Main.OnLoop.Add(player, true);
            }
            if (g_Main.OnLoop.ContainsKey(player))
            {
                return HookResult.Continue;
            }
        }

        if(Configs.GetConfigData().RemoveDefaultDisconnect == 2)
        {
            if (g_Main.Player_Data.ContainsKey(player))
            {
                g_Main.Player_Data[player].Remove_Icon = true;
            }
        }

        _ = HandlePlayerConnectionsAsync(player, true, reason);

        return HookResult.Continue;
    }

    public void OnMapEnd()
    {
        try
        {
            Helper.ClearVariables();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Map end cleanup error: {ex.Message}");
        }
    }

    public override void Unload(bool hotReload)
    {
        CustomHooks.CleanUp();

        try
        {
            Helper.ClearVariables();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Unload cleanup error: {ex.Message}");
        }
    }


    /* [ConsoleCommand("css_test", "test")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void test(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(!player.IsValid())return;
        
    } */
    
}