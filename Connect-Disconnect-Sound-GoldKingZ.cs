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
    public override string ModuleVersion => "1.1.4";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    public static MainPlugin Instance { get; set; } = new();
    public Globals g_Main = new();
    private readonly SayText2 OnSayText2 = new();
    
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
        
        HookUserMessage(118, OnUserMessage_OnSayText2, HookMode.Pre);

        Helper.RegisterCssCommands(Configs.GetConfigData().Toggle_Messages_CommandsInGame.GetCommands(), "Commands To Enable/Disable Messages", OnSayText2.CommandsAction_Messages);
        Helper.RegisterCssCommands(Configs.GetConfigData().Toggle_Sounds_CommandsInGame.GetCommands(), "Commands To Enable/Disable Sounds", OnSayText2.CommandsAction_Sounds);

        if (hotReload)
        {
            Helper.LoadJson();
            g_Main.ServerPublicIpAdress = ConVar.Find("ip")?.StringValue!;
            g_Main.ServerPort = ConVar.Find("hostport")?.GetPrimitiveValue<int>().ToString()!;

            if (Configs.GetConfigData().Log_Locally_AutoDeleteLogsMoreThanXdaysOld > 0)
            {
                string Fpath = Path.Combine(ModuleDirectory, "logs");
                Helper.DeleteOldFiles(Fpath, "*" + ".txt", TimeSpan.FromDays(Configs.GetConfigData().Log_Locally_AutoDeleteLogsMoreThanXdaysOld));
            }

            if (string.IsNullOrEmpty(g_Main.ServerPublicIpAdress) || Configs.GetConfigData().MySql_Enable)
            {
                _ = Task.Run(async () =>
                {
                    if (string.IsNullOrEmpty(g_Main.ServerPublicIpAdress))
                    {
                        string ip = await Helper.GetPublicIp();
                        if (!string.IsNullOrEmpty(ip))
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

            foreach (var players in Helper.GetPlayersController(false, false, false, true, true, true))
            {
                if (!players.IsValid()) continue;
                Helper.CheckPlayerInGlobals(players);
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
                if (line.TrimStart().StartsWith("//")) continue;
                manifest.AddResource(line);
                Helper.DebugMessage("ResourceManifest : " + line);
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

        if (Configs.GetConfigData().Log_Locally_AutoDeleteLogsMoreThanXdaysOld > 0)
        {
            string Fpath = Path.Combine(ModuleDirectory, "logs");
            Helper.DeleteOldFiles(Fpath, "*" + ".txt", TimeSpan.FromDays(Configs.GetConfigData().Log_Locally_AutoDeleteLogsMoreThanXdaysOld));
        }

        if (string.IsNullOrEmpty(g_Main.ServerPublicIpAdress) || Configs.GetConfigData().MySql_Enable)
        {
            _ = Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(g_Main.ServerPublicIpAdress))
                {
                    string ip = await Helper.GetPublicIp();
                    if (!string.IsNullOrEmpty(ip))
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
        
        if (Configs.GetConfigData().EarlyConnection) return HookResult.Continue;

        _ = HandlePlayerConnectionsAsync(player, false, "");

        return HookResult.Continue;
    }

    private HookResult OnEventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        var victim = @event.Userid;
        if(!victim.IsValid())return HookResult.Continue;

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
            var (playersteamId2, playersteamId3, playersteamId32, playersteamId64) = player.SteamID.GetPlayerSteamID();
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
                        Helper.AdvancedPlayerPrintToChat(allplayers, null!, formatted);
                    }
                    
                    if (ConnectionSettingsSound.Count > 0 && (g_Main.Player_Data[allplayers].Toggle_Sounds == 1 || g_Main.Player_Data[allplayers].Toggle_Sounds == -1))
                    {
                        string nextSound = ConnectionSettingsSound.GetNextSound(pickRandom: Configs.GetConfigData().PickRandomSounds);

                        if(nextSound.StartsWith("sounds/"))
                        {
                            allplayers.ExecuteClientCommand($"play {nextSound}");
                        }else
                        {
                            float SoundVolume = ConnectionSettingsSoundVolume!.ToPercentageFloat();
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

    private HookResult OnUserMessage_OnSayText2(CounterStrikeSharp.API.Modules.UserMessages.UserMessage um)
    {
        var entityindex = um.ReadInt("entityindex");
        var player = Utilities.GetPlayerFromIndex(entityindex);
        if (!player.IsValid()) return HookResult.Continue;
        Helper.CheckPlayerInGlobals(player);

        var message = um.ReadString("param2");
        if (string.IsNullOrWhiteSpace(message)) return HookResult.Continue;

        message = message.Trim();

        OnSayText2.OnSayText2(um, player, message);
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
        
        if (!string.IsNullOrEmpty(Configs.GetConfigData().IgnoreTheseDisconnectReasons))
        {
            var ignoreReasons = Configs.GetConfigData().IgnoreTheseDisconnectReasons
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToList();

            if (ignoreReasons.Contains(reasonInt.ToString()))
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
        if (!player.IsValid()) return;
    } */
}