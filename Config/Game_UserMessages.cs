using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Text;
using CnD_Sound.Config;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Translations;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;

namespace CnD_Sound;

public class Game_UserMessages
{
    public HookResult HookPlayerChat_UserMessages(CounterStrikeSharp.API.Modules.UserMessages.UserMessage? um, CCSPlayerController? player, string message)
    {
        if (!player.IsValid()) return HookResult.Continue;

        var g_Main = MainPlugin.Instance.g_Main;
        Helper.CheckPlayerInGlobals(player);

        if (!g_Main.Player_Data.TryGetValue(player.Slot, out var playerData)) return HookResult.Continue;

        bool onetime = (DateTime.Now - playerData.EventPlayerChat).TotalSeconds > 0.4;
        if (onetime)
        {
            playerData.EventPlayerChat = DateTime.Now;
        }


        if (Configs.Instance.Reload_CnD.Reload_CnD_CommandsInGame.ConvertCommands(true)?.Any(c => message.Equals(c.Trim(), StringComparison.OrdinalIgnoreCase)) == true)
        {
            if (Configs.Instance.Reload_CnD.Reload_CnD_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, Configs.Instance.Reload_CnD.Reload_CnD_Flags))
            {
                if (onetime) Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Not.Allowed"]);
            }
            else
            {
                if (onetime)
                {
                    Helper.ClearVariables(true);
                    Helper.RemoveRegisterCommandsAndHooks();

                    Configs.Load(MainPlugin.Instance.ModuleDirectory);
                    _ = Task.Run(Helper.DownloadMissingFilesAsync);

                    Helper.LoadJson_connect_disconnect_config(true, player);
                    Helper.LoadJson_disconnect_reasons(true, player);
                    Helper.RegisterCommandsAndHooks();
                    Helper.ReloadPlayersGlobals();

                    MainPlugin.Instance.g_Main.ServerPort = ConVar.Find("hostport")?.GetPrimitiveValue<int>().ToString()!;
                    
                    if (Configs.Instance.Log_Locally_AutoDeleteLogsMoreThanXdaysOld > 0)
                    {
                        string Fpath = Path.Combine(MainPlugin.Instance.ModuleDirectory, "logs");
                        Helper.DeleteOldFiles(Fpath, "*" + ".txt", TimeSpan.FromDays(Configs.Instance.Log_Locally_AutoDeleteLogsMoreThanXdaysOld));
                    }

                    MainPlugin.Instance.AddTimer(3.0f, async () =>
                    {
                        bool success = false;
                        string getip = Helper.GetServerIp();

                        if (!string.IsNullOrEmpty(getip))
                        {
                            MainPlugin.Instance.g_Main.ServerPublicIpAdress = getip;
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
                                MainPlugin.Instance.g_Main.ServerPublicIpAdress = getip_2;
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
                    
                    Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Successfully"]);
                }
                Helper.MuteCommands(um, Configs.Instance.Reload_CnD.Reload_CnD_Hide);
            }
            Helper.MuteCommands(um, Configs.Instance.Reload_CnD.Reload_CnD_Hide, true);
        }

        if (Configs.Instance.CnD_Sounds.CnDSounds > 1)
        {
            if (Configs.Instance.CnD_Sounds.CnDSounds_CommandsInGame.ConvertCommands(true)?.Any(c => message.Equals(c.Trim(), StringComparison.OrdinalIgnoreCase)) == true)
            {
                if (Configs.Instance.CnD_Sounds.CnDSounds_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, Configs.Instance.CnD_Sounds.CnDSounds_Flags))
                {
                    if (onetime) Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Sounds.Not.Allowed"]);
                }
                else
                {
                    if (onetime)
                    {
                        playerData.Toggle_Sounds = playerData.Toggle_Sounds.ToggleOnOff();
                        if (playerData.Toggle_Sounds == -1)
                        {
                            Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Sounds.Enabled"]);
                        }
                        else if (playerData.Toggle_Sounds == -2)
                        {
                            Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Sounds.Disabled"]);
                        }
                    }
                    Helper.MuteCommands(um, Configs.Instance.CnD_Sounds.CnDSounds_Hide);
                }
                Helper.MuteCommands(um, Configs.Instance.CnD_Sounds.CnDSounds_Hide, true);
            }
        }
        

        if (Configs.Instance.CnD_Messages.CnDMessages > 1)
        {
            if (Configs.Instance.CnD_Messages.CnDMessages_CommandsInGame.ConvertCommands(true)?.Any(c => message.Equals(c.Trim(), StringComparison.OrdinalIgnoreCase)) == true)
            {
                if (Configs.Instance.CnD_Messages.CnDMessages_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, Configs.Instance.CnD_Messages.CnDMessages_Flags))
                {
                    if (onetime) Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Messages.Not.Allowed"]);
                }
                else
                {
                    if (onetime)
                    {
                        playerData.Toggle_Messages = playerData.Toggle_Messages.ToggleOnOff();
                        if (playerData.Toggle_Messages == -1)
                        {
                            Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Messages.Enabled"]);
                        }
                        else if (playerData.Toggle_Messages == -2)
                        {
                            Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Messages.Disabled"]);
                        }
                    }
                    Helper.MuteCommands(um, Configs.Instance.CnD_Messages.CnDMessages_Hide);
                }
                Helper.MuteCommands(um, Configs.Instance.CnD_Messages.CnDMessages_Hide, true);
            }
        }

        return HookResult.Continue;
    }














    #region Commands Hook

    public void CommandsAction_ReloadPlugin(CCSPlayerController? player, CommandInfo info)
    {
        if (!player.IsValid()) return;

        Helper.CheckPlayerInGlobals(player);

        if (!MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player.Slot, out var playerData)) return;
        if ((DateTime.Now - playerData.EventPlayerChat).TotalSeconds <= 0.4) return;

        if (Configs.Instance.Reload_CnD.Reload_CnD_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, Configs.Instance.Reload_CnD.Reload_CnD_Flags))
        {
            Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Not.Allowed"]);
        }
        else
        {
            Helper.ClearVariables(true);
            Helper.RemoveRegisterCommandsAndHooks();

            Configs.Load(MainPlugin.Instance.ModuleDirectory);
            _ = Task.Run(Helper.DownloadMissingFilesAsync);

            Helper.LoadJson_connect_disconnect_config(true, player, info);
            Helper.LoadJson_disconnect_reasons(true, player, info);
            Helper.RegisterCommandsAndHooks();
            Helper.ReloadPlayersGlobals();

            MainPlugin.Instance.g_Main.ServerPort = ConVar.Find("hostport")?.GetPrimitiveValue<int>().ToString()!;
            
            if (Configs.Instance.Log_Locally_AutoDeleteLogsMoreThanXdaysOld > 0)
            {
                string Fpath = Path.Combine(MainPlugin.Instance.ModuleDirectory, "logs");
                Helper.DeleteOldFiles(Fpath, "*" + ".txt", TimeSpan.FromDays(Configs.Instance.Log_Locally_AutoDeleteLogsMoreThanXdaysOld));
            }

            MainPlugin.Instance.AddTimer(3.0f, async () =>
            {
                bool success = false;
                string getip = Helper.GetServerIp();

                if (!string.IsNullOrEmpty(getip))
                {
                    MainPlugin.Instance.g_Main.ServerPublicIpAdress = getip;
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
                        MainPlugin.Instance.g_Main.ServerPublicIpAdress = getip_2;
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

            Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Successfully"]);
        }
    }

    public void CommandsAction_Toggle_Sounds(CCSPlayerController? player, CommandInfo info)
    {
        if (Configs.Instance.CnD_Sounds.CnDSounds < 2 || !player.IsValid()) return;

        Helper.CheckPlayerInGlobals(player);

        if (!MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player.Slot, out var playerData)) return;
        if ((DateTime.Now - playerData.EventPlayerChat).TotalSeconds <= 0.4) return;

        if (Configs.Instance.CnD_Sounds.CnDSounds_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, Configs.Instance.CnD_Sounds.CnDSounds_Flags))
        {
            Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Sounds.Not.Allowed"]);
        }
        else
        {
            playerData.Toggle_Sounds = playerData.Toggle_Sounds.ToggleOnOff();
            if (playerData.Toggle_Sounds == -1)
            {
                Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Sounds.Enabled"]);
            }
            else if (playerData.Toggle_Sounds == -2)
            {
                Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Sounds.Disabled"]);
            }
        }
    }

    public void CommandsAction_Toggle_Messages(CCSPlayerController? player, CommandInfo info)
    {
        if (Configs.Instance.CnD_Messages.CnDMessages < 2 || !player.IsValid()) return;

        Helper.CheckPlayerInGlobals(player);

        if (!MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player.Slot, out var playerData)) return;
        if ((DateTime.Now - playerData.EventPlayerChat).TotalSeconds <= 0.4) return;

        if (Configs.Instance.CnD_Messages.CnDMessages_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, Configs.Instance.CnD_Messages.CnDMessages_Flags))
        {
            Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Messages.Not.Allowed"]);
        }
        else
        {
            playerData.Toggle_Messages = playerData.Toggle_Messages.ToggleOnOff();
            if (playerData.Toggle_Messages == -1)
            {
                Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Messages.Enabled"]);
            }
            else if (playerData.Toggle_Messages == -2)
            {
                Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintToChatToPlayer.Toggle.Messages.Disabled"]);
            }
        }
    }

    #endregion Commands Hook
}