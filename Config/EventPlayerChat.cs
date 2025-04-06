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

public class PlayerChat
{
    public HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info, bool TeamChat)
	{
        if (!player.IsValid())return HookResult.Continue;
        var playerid = player.SteamID;
        var eventmessage = info.ArgString;
        eventmessage = eventmessage.TrimStart('"');
        eventmessage = eventmessage.TrimEnd('"');
        
        if (string.IsNullOrWhiteSpace(eventmessage)) return HookResult.Continue;
        string trimmedMessageStart = eventmessage.TrimStart();
        string message = trimmedMessageStart.TrimEnd();

        string[] Toggle_Messages_CommandsInGames = Configs.GetConfigData().Toggle_Messages_CommandsInGame.Split(',');
        if (Toggle_Messages_CommandsInGames.Any(command => message.Equals(command.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            if(!string.IsNullOrEmpty(Configs.GetConfigData().Toggle_Messages_Flags) && !Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().Toggle_Messages_Flags))
            {
                Helper.AdvancedPlayerPrintToChat(player, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Messages.Not.Allowed"]);
            }else
            {
                if(MainPlugin.Instance.g_Main.Player_Data.ContainsKey(player))
                {
                    MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Messages = MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Messages.ToggleOnOff();

                    if(MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Messages == -1)
                    {
                        Helper.AdvancedPlayerPrintToChat(player, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Messages.Enabled"]);
                    }else if(MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Messages == -2)
                    {
                        Helper.AdvancedPlayerPrintToChat(player, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Messages.Disabled"]);
                    }
                }
            }
        }

        string[] Toggle_Sounds_CommandsInGames = Configs.GetConfigData().Toggle_Sounds_CommandsInGame.Split(',');
        if (Toggle_Sounds_CommandsInGames.Any(command => message.Equals(command.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            if(!string.IsNullOrEmpty(Configs.GetConfigData().Toggle_Sounds_Flags) && !Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().Toggle_Sounds_Flags))
            {
                Helper.AdvancedPlayerPrintToChat(player, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Sounds.Not.Allowed"]);
            }else
            {
                if(MainPlugin.Instance.g_Main.Player_Data.ContainsKey(player))
                {
                    MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Sounds = MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Sounds.ToggleOnOff();

                    if(MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Sounds == -1)
                    {
                        Helper.AdvancedPlayerPrintToChat(player, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Sounds.Enabled"]);
                    }else if(MainPlugin.Instance.g_Main.Player_Data[player].Toggle_Sounds == -2)
                    {
                        Helper.AdvancedPlayerPrintToChat(player, MainPlugin.Instance.Localizer["PrintChatToPlayer.Toggle.Sounds.Disabled"]);
                    }
                }
            }
        }

        return HookResult.Continue;
    }
}