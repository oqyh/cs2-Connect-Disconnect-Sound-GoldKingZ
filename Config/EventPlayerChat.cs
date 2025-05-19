using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;
using MySqlConnector;
using CnD_Sound.Config;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CounterStrikeSharp.API.Modules.Entities;
using System;
using System.Globalization;
using System.Drawing;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using System.Runtime.InteropServices;
using System.Reflection.Metadata.Ecma335;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Core.Translations;

namespace CnD_Sound;

public class SayText2
{
    public HookResult OnSayText2(CounterStrikeSharp.API.Modules.UserMessages.UserMessage um, CCSPlayerController? player, string message)
    {
        if (!player.IsValid() || !MainPlugin.Instance.g_Main.Player_Data.ContainsKey(player)) return HookResult.Continue;

        if (Configs.GetConfigData().Toggle_Messages_CommandsInGame.GetCommands(true).Any(command => message.Equals(command.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            if (!string.IsNullOrEmpty(Configs.GetConfigData().Toggle_Messages_Flags) && !Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().Toggle_Messages_Flags))
            {
                if (!message.StartsWith("!")) Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Messages.Not.Allowed"]);
            }
            else
            {
                if (!message.StartsWith("!"))
                {
                    MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Messages = MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Messages.ToggleOnOff();
                    if (MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Messages == -1)
                    {
                        Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Messages.Enabled"]);
                    }
                    else if (MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Messages == -2)
                    {
                        Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Messages.Disabled"]);
                    }
                }
                if (Configs.GetConfigData().Toggle_Messages_Hide)
                {
                    um.Recipients.Clear();
                }
            }
        }

        if ( Configs.GetConfigData().Toggle_Sounds_CommandsInGame.GetCommands(true).Any(command => message.Equals(command.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            if (!string.IsNullOrEmpty(Configs.GetConfigData().Toggle_Sounds_Flags) && !Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().Toggle_Sounds_Flags))
            {
                if (!message.StartsWith("!")) Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Sounds.Not.Allowed"]);
            }
            else
            {
                if (!message.StartsWith("!"))
                {
                    MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Sounds = MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Sounds.ToggleOnOff();
                    if (MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Sounds == -1)
                    {
                        Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Sounds.Enabled"]);
                    }
                    else if (MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Sounds == -2)
                    {
                        Helper.AdvancedPlayerPrintToChat(player, null!, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Sounds.Disabled"]);
                    }
                }
                if (Configs.GetConfigData().Toggle_Sounds_Hide)
                {
                    um.Recipients.Clear();
                }
            }
        }

        return HookResult.Continue;
    }
    

    public void CommandsAction_Sounds(CCSPlayerController? player, CommandInfo info)
    {
        if (!player.IsValid()) return;

        Helper.CheckPlayerInGlobals(player);

        if (!MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player, out var playerData)) return;

        if (!string.IsNullOrEmpty(Configs.GetConfigData().Toggle_Sounds_Flags) && !Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().Toggle_Sounds_Flags))
        {
            Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Sounds.Not.Allowed"]);
        }
        else
        {
            playerData.Toggle_Sounds = playerData.Toggle_Sounds.ToggleOnOff();
            if (playerData.Toggle_Sounds == -1)
            {
                Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Sounds.Enabled"]);
            }
            else if (playerData.Toggle_Sounds == -2)
            {
                Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Sounds.Disabled"]);
            }
        }
    }

    public void CommandsAction_Messages(CCSPlayerController? player, CommandInfo info)
    {
        if (!player.IsValid()) return;

        Helper.CheckPlayerInGlobals(player);

        if (!MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player, out var playerData)) return;

        if (!string.IsNullOrEmpty(Configs.GetConfigData().Toggle_Messages_Flags) && !Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().Toggle_Messages_Flags))
        {
            Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Messages.Not.Allowed"]);
        }
        else
        {
            playerData.Toggle_Messages = playerData.Toggle_Messages.ToggleOnOff();
            if (playerData.Toggle_Messages == -1)
            {
                Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Messages.Enabled"]);
            }
            else if (playerData.Toggle_Messages == -2)
            {
                Helper.AdvancedPlayerPrintToChat(player, info, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Messages.Disabled"]);
            }
        }
    }
}