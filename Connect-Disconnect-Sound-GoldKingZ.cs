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
using CounterStrikeSharp.API.Modules.Admin;
using CnD_Sound.Config;
using System.Text;

namespace CnD_Sound;

public class MainPlugin : BasePlugin
{
    public override string ModuleName => "Connect Disconnect Sound (Continent , Country , City , Message , Sounds , Logs , Discord)";
    public override string ModuleVersion => "1.1.7";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    public static MainPlugin Instance { get; set; } = new();
    public Globals g_Main = new();
    public readonly Game_UserMessages Game_UserMessages = new();
    
    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory);
        
        Helper.RemoveRegisterCommandsAndHooks();

        _ = Task.Run(Helper.DownloadMissingFilesAsync);
        
        Helper.LoadJson_connect_disconnect_config();
        Helper.LoadJson_disconnect_reasons();
        Helper.RegisterCommandsAndHooks();

        if (hotReload)
        {
            Helper.RemoveRegisterCommandsAndHooks();

            _ = Task.Run(Helper.DownloadMissingFilesAsync);
            
            Helper.LoadJson_connect_disconnect_config();
            Helper.LoadJson_disconnect_reasons();
            Helper.RegisterCommandsAndHooks();
            Helper.ReloadPlayersGlobals();

            g_Main.ServerPort = ConVar.Find("hostport")?.GetPrimitiveValue<int>().ToString()!;

            if (Configs.Instance.Log_Locally_AutoDeleteLogsMoreThanXdaysOld > 0)
            {
                string Fpath = Path.Combine(ModuleDirectory, "logs");
                Helper.DeleteOldFiles(Fpath, "*" + ".txt", TimeSpan.FromDays(Configs.Instance.Log_Locally_AutoDeleteLogsMoreThanXdaysOld));
            }

            AddTimer(3.0f, async () =>
            {
                bool success = false;
                string getip = Helper.GetServerIp();

                if (!string.IsNullOrEmpty(getip))
                {
                    g_Main.ServerPublicIpAdress = getip;
                    try
                    {
                        success = true;
                    }
                    catch { }
                }

                if (!success)
                {
                    string getip_2 = await Helper.GetPublicIp();
                    if (!string.IsNullOrEmpty(getip_2))
                    {
                        g_Main.ServerPublicIpAdress = getip_2;
                        try
                        {
                            success = true;
                        }
                        catch { }
                    }
                }

                if (Configs.Instance.MySql_Enable > 0)
                {
                    await MySqlDataManager.CreateTableIfNotExistsAsync();
                }
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }
    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        try
        {
            if(Configs.Instance.AutoPrecacheResources)
            {
                foreach (var Events in VpkParser.GetEventsFromVpk())
                {
                    var folders = Configs.Instance.AutoPrecacheResources_Folders;

                    if (folders.Count == 0 || folders.Any(f => Events.StartsWith(f)))
                    {
                        manifest.AddResource(Events);
                        Helper.DebugMessage("Auto ResourceManifest : " + Events);
                    }
                }
            }

            string filePath = Path.Combine(ModuleDirectory, "config/ServerPrecacheResources.txt");
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (line.TrimStart().StartsWith("//")) continue;
                manifest.AddResource(line);
                Helper.DebugMessage("Manual ResourceManifest : " + line);
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"OnServerPrecacheResources Error: {ex.Message}");
        }
    }

    public void OnMapStart(string Map)
    {
        Helper.RemoveRegisterCommandsAndHooks();

        _ = Task.Run(Helper.DownloadMissingFilesAsync);
        
        Helper.LoadJson_connect_disconnect_config();
        Helper.LoadJson_disconnect_reasons();
        Helper.RegisterCommandsAndHooks();

        g_Main.ServerPort = ConVar.Find("hostport")?.GetPrimitiveValue<int>().ToString()!;

        if (Configs.Instance.Log_Locally_AutoDeleteLogsMoreThanXdaysOld > 0)
        {
            string Fpath = Path.Combine(ModuleDirectory, "logs");
            Helper.DeleteOldFiles(Fpath, "*" + ".txt", TimeSpan.FromDays(Configs.Instance.Log_Locally_AutoDeleteLogsMoreThanXdaysOld));
        }

        
        AddTimer(3.0f, async () =>
        {
            bool success = false;
            string getip = Helper.GetServerIp();

            if (!string.IsNullOrEmpty(getip))
            {
                g_Main.ServerPublicIpAdress = getip;
                try
                {
                    success = true;
                }
                catch { }
            }

            if (!success)
            {
                string getip_2 = await Helper.GetPublicIp();
                if (!string.IsNullOrEmpty(getip_2))
                {
                    g_Main.ServerPublicIpAdress = getip_2;
                    try
                    {
                        success = true;
                    }
                    catch { }
                }
            }

            if (Configs.Instance.MySql_Enable > 0)
            {
                await MySqlDataManager.CreateTableIfNotExistsAsync();
            }
        }, TimerFlags.STOP_ON_MAPCHANGE);
        
    }

    public void OnClientAuthorized(int playerSlot, SteamID steamId)
    {
        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if (!player.IsValid()) return;

        if (g_Main.Player_Disconnect_Reasons.TryGetValue(player.Slot, out var handle))
        {
            if(handle.OneTime)return;
        }

        if (Configs.Instance.EarlyConnection)
        {
            _ = HandlePlayerConnectionsAsync(player, false, "");

            if (g_Main.Player_Disconnect_Reasons.ContainsKey(player.Slot))
            {
                g_Main.Player_Disconnect_Reasons[player.Slot].OneTime = true;
            }
        }
    }
    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;

        var player = @event.Userid;
        if (!player.IsValid())return HookResult.Continue;

        if (g_Main.Player_Disconnect_Reasons.ContainsKey(player.Slot))
        {
            g_Main.Player_Disconnect_Reasons.Remove(player.Slot);
        }

        if (!Configs.Instance.EarlyConnection)
        {
            _ = HandlePlayerConnectionsAsync(player, false, "");
        }
        
        return HookResult.Continue;
    }

    public HookResult OnEventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        var victim = @event.Userid;
        if(!victim.IsValid())return HookResult.Continue;

        if (Configs.Instance.RemoveDefaultDisconnect == 2)
        {
            if (victim.Connected == PlayerConnectedState.PlayerDisconnecting)
            {
                info.DontBroadcast = true;
            }
        }
        
        return HookResult.Continue;
    }

    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;

        if (Configs.Instance.RemoveDefaultDisconnect > 0)
        {
            info.DontBroadcast = true;
        }

        var player = @event.Userid;
        var reasonInt = @event.Reason;
        var reason = Helper.GetDisconnectReason(reasonInt);

        if (!player.IsValid())return HookResult.Continue;

        if (Configs.Instance.MySql_Enable == 1 || Configs.Instance.Cookies_Enable == 1)
        {
            _ = HandlePlayerDisconnectAsync(player);
        }

        var ignoreReasons = Configs.Instance.IgnoreTheseDisconnectReasons;
        if (ignoreReasons != null && ignoreReasons.Count > 0)
        {
            if (ignoreReasons.Contains(reasonInt))
            {
                if (g_Main.Player_Disconnect_Reasons.TryGetValue(player.Slot, out var existingEntry))
                {
                    if (!existingEntry.Player_Disconnect_Reasons.Contains(reason))
                    {
                        existingEntry.Player_Disconnect_Reasons.Add(reason);
                    }
                }
                else
                {
                    var reasons = new HashSet<string> { reason };
                    g_Main.Player_Disconnect_Reasons[player.Slot] = new Globals.Player_Disconnect_ReasonsClass(player, reasons, false);
                }

                return HookResult.Continue;
            }
        }
        
        _ = HandlePlayerConnectionsAsync(player, true, reason);

        return HookResult.Continue;
    }

    public async Task HandlePlayerConnectionsAsync(CCSPlayerController Getplayer, bool Disconnect, string reason, bool mutemessage = false)
    {
        try
        {
            var player = Getplayer;
            if (!player.IsValid()) return;

            var playername = player.PlayerName.RemoveColorNames();
            var (playersteamId2, playersteamId3, playersteamId32, playersteamId64) = player.SteamID.GetPlayerSteamID();
            var playerip = player.IpAddress?.Split(':')[0] ?? Localizer["InValidIpAddress"];

            if(!Disconnect)
            {
                await Helper.LoadPlayerData(player);
            }

            var geoInfo = await Helper.GetGeoInfoAsync(playerip);

            if (Configs.Instance.AutoSetPlayerLanguage)
            {
                Server.NextFrame(() =>
                {
                    if (player.IsValid())
                    {
                        Helper.SetPlayerLanguage(player, geoInfo.CountryCode);
                    }
                });
            }

            if(!mutemessage)
            {
                await Server.NextFrameAsync(async () =>
                {
                    if (!player.IsValid() || !g_Main.Player_Data.ContainsKey(player.Slot))return;

                    var (ConnectionSettingsMessage, ConnectionSettingsSound, ConnectionSettingsSoundVolume) = Helper.GetPlayerConnectionSettings(player, Disconnect?"DISCONNECT":"CONNECT");
                    string formatted = "";                

                    if (!string.IsNullOrEmpty(ConnectionSettingsMessage))
                    {
                        formatted = ConnectionSettingsMessage.ReplaceMessages(
                            DateTime.Now.ToString(Configs.Instance.DateFormat),
                            DateTime.Now.ToString(Configs.Instance.TimeFormat),
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

                    string nextSound = ConnectionSettingsSound.Count > 0? ConnectionSettingsSound.GetSoundPath(Configs.Instance.PickSoundsByOrder) : "";

                    foreach (var allplayers in Helper.GetPlayersController())
                    {
                        if (!allplayers.IsValid() || !g_Main.Player_Data.ContainsKey(allplayers.Slot))continue;

                        if (!string.IsNullOrEmpty(formatted) && (g_Main.Player_Data[allplayers.Slot].Toggle_Messages == 1 || g_Main.Player_Data[allplayers.Slot].Toggle_Messages == -1 || Configs.Instance.CnD_Messages.CnDMessages == 1))
                        {
                            formatted = formatted.ReplaceColorTags();
                            Helper.AdvancedPlayerPrintToChat(allplayers, null!, formatted);
                        }
                        
                        if (!string.IsNullOrEmpty(nextSound) && (g_Main.Player_Data[allplayers.Slot].Toggle_Sounds == 1 || g_Main.Player_Data[allplayers.Slot].Toggle_Sounds == -1 || Configs.Instance.CnD_Sounds.CnDSounds == 1))
                        {
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

                    if (Configs.Instance.Log_Locally_Enable)
                    {
                        var logPath = Path.Combine(ModuleDirectory, "logs");
                        Directory.CreateDirectory(logPath);
                        
                        var fileName = DateTime.Now.ToString(Configs.Instance.Log_Locally_DateFormat) + ".txt";
                        var fullPath = Path.Combine(logPath, fileName);
                        string Format = Disconnect?Configs.Instance.Log_Locally_Disconnect_Format:Configs.Instance.Log_Locally_Connect_Format;
                        var logMessage = Format?.ReplaceMessages(
                            DateTime.Now.ToString(Configs.Instance.Log_Locally_DateFormat),
                            DateTime.Now.ToString(Configs.Instance.Log_Locally_TimeFormat),
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

                if (Disconnect?!string.IsNullOrEmpty(Configs.Instance.Discord_Disconnect_WebHook):!string.IsNullOrEmpty(Configs.Instance.Discord_Connect_WebHook))
                {
                    string Format = Disconnect?Configs.Instance.Discord_Disconnect_Format:Configs.Instance.Discord_Connect_Format;
                    var discordMessage = Format?.ReplaceMessages(
                        DateTime.Now.ToString(Configs.Instance.Discord_DateFormat),
                        DateTime.Now.ToString(Configs.Instance.Discord_TimeFormat),
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
                            Disconnect?Configs.Instance.Discord_Disconnect_Style:Configs.Instance.Discord_Connect_Style,
                            Disconnect?Configs.Instance.Discord_Disconnect_WebHook:Configs.Instance.Discord_Connect_WebHook,
                            discordMessage,
                            playersteamId64,
                            playername,
                            $"{g_Main.ServerPublicIpAdress}:{g_Main.ServerPort}",
                            Disconnect
                        );
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"HandlePlayerConnectionsAsync error: {ex.Message}");
        }
    }

    public async Task HandlePlayerDisconnectAsync(CCSPlayerController player)
    {
        try
        {
            if (!player.IsValid()) return;
            await Helper.SavePlayerDataOnDisconnect(player);
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"HandlePlayerDisconnectAsync error: {ex.Message}");
        }
    }

    public HookResult OnPlayerSay(CCSPlayerController? player, CommandInfo info)
    {
        if (!player.IsValid()) return HookResult.Continue;

        var eventmessage = info.ArgString;
        eventmessage = eventmessage.TrimStart('"');
        eventmessage = eventmessage.TrimEnd('"');
        if (string.IsNullOrWhiteSpace(eventmessage)) return HookResult.Continue;

        string message = eventmessage.Trim();

        Game_UserMessages.HookPlayerChat_UserMessages(null, player, message);

        return HookResult.Continue;
    }
    public HookResult OnPlayerSay_Team(CCSPlayerController? player, CommandInfo info)
    {
        if (!player.IsValid()) return HookResult.Continue;

        var eventmessage = info.ArgString;
        eventmessage = eventmessage.TrimStart('"');
        eventmessage = eventmessage.TrimEnd('"');
        if (string.IsNullOrWhiteSpace(eventmessage)) return HookResult.Continue;

        string message = eventmessage.Trim();

        Game_UserMessages.HookPlayerChat_UserMessages(null, player, message);

        return HookResult.Continue;
    }
    public HookResult OnUserMessage_OnSayText2(CounterStrikeSharp.API.Modules.UserMessages.UserMessage um)
    {
        var entityindex = um.ReadInt("entityindex");
        var player = Utilities.GetPlayerFromIndex(entityindex);
        if (!player.IsValid()) return HookResult.Continue;

        var eventmessage_Bytes = um.ReadBytes("param2");
        var eventmessage = Encoding.UTF8.GetString(eventmessage_Bytes);
        if (string.IsNullOrWhiteSpace(eventmessage)) return HookResult.Continue;

        string message = eventmessage.Trim();
        Game_UserMessages.HookPlayerChat_UserMessages(um, player, message);

        return HookResult.Continue;
    }

    public void OnMapEnd()
    {
        try
        {
            if (Configs.Instance.MySql_Enable > 0 || Configs.Instance.Cookies_Enable > 0)
            {
                Helper.SavePlayersValues();
            }

            Helper.ClearVariables();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"OnMapEnd Error: {ex.Message}");
        }
    }

    public override void Unload(bool hotReload)
    {
        try
        {
            Helper.RemoveRegisterCommandsAndHooks();
            Helper.ClearVariables(true);

        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Unload Error: {ex.Message}");
        }

        if (hotReload)
        {
            try
            {
                Helper.RemoveRegisterCommandsAndHooks();
                Helper.ClearVariables(true);
            }
            catch (Exception ex)
            {
                Helper.DebugMessage($"Unload hotReload Error: {ex.Message}");
            }
        }
    }
    

    [ConsoleCommand("gkz_download", "Force Download Addons")]
    [ConsoleCommand("gkz_forcedownload", "Force Download Addons")]
    [RequiresPermissions("@css/root")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void ForceUpdate(CCSPlayerController? player, CommandInfo commandInfo)
    {
        string command = commandInfo.GetArg(1);
        if(string.IsNullOrEmpty(command))
        {
            commandInfo.ReplyToCommand("Missing <WorkShopID>");
            return;
        }

        AddTimer(1.0f, () =>
        {
            Server.ExecuteCommand("mm_remove_addon " + command);
            AddTimer(0.5f, () =>
            {
                Server.ExecuteCommand("mm_add_addon " + command);
                AddTimer(0.10f, () =>
                {
                    Server.ExecuteCommand("mm_download_addon " + command);
                }, TimerFlags.STOP_ON_MAPCHANGE);
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }, TimerFlags.STOP_ON_MAPCHANGE);
    }

    /* [ConsoleCommand("css_test", "testttt")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void test(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (!player.IsValid()) return;
    } */
}