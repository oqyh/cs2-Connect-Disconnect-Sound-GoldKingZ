using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Core.Translations;
using Newtonsoft.Json;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using Newtonsoft.Json.Linq;
using CnD_Sound.Config;
using System.Globalization;
using CounterStrikeSharp.API.Modules.Entities;

namespace CnD_Sound;

public class Helper
{
    public static void RegisterCssCommands(string[]? commands, string description, CommandInfo.CommandCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrEmpty(cmd)) continue;
            MainPlugin.Instance.AddCommand(cmd, description, callback);
        }
    }


    public static void RemoveCssCommands(string[]? commands, CommandInfo.CommandCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrEmpty(cmd)) continue;
            MainPlugin.Instance.RemoveCommand(cmd, callback);
        }
    }

    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, CounterStrikeSharp.API.Modules.Commands.CommandInfo commandInfo, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i]?.ToString() ?? "");
        }

        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    if (commandInfo != null && commandInfo.CallingContext == CounterStrikeSharp.API.Modules.Commands.CommandCallingContext.Console)
                    {
                        player.PrintToConsole(" " + trimmedPart);
                    }
                    else
                    {
                        player.PrintToChat(" " + trimmedPart);
                    }
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            if (commandInfo != null && commandInfo.CallingContext == CounterStrikeSharp.API.Modules.Commands.CommandCallingContext.Console)
            {
                player.PrintToConsole(message);
            }
            else
            {
                player.PrintToChat(message);
            }
        }
    }
    public static void AdvancedPlayerPrintToConsole(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToConsole(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToConsole(message);
        }
    }
    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    Server.PrintToChatAll(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            Server.PrintToChatAll(message);
        }
    }
    
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        if (string.IsNullOrEmpty(groups) || player == null || !player.IsValid)
            return false;

        return groups.Split('|')
            .Select(segment => segment.Trim())
            .Any(trimmedSegment => Permission_CheckPermissionSegment(player, trimmedSegment));
    }

    private static bool Permission_CheckPermissionSegment(CCSPlayerController player, string segment)
    {
        if (string.IsNullOrEmpty(segment)) return false;

        int colonIndex = segment.IndexOf(':');
        if (colonIndex == -1 || colonIndex == 0) return false;

        string prefix = segment.Substring(0, colonIndex).Trim().ToLower();
        string values = segment.Substring(colonIndex + 1).Trim();

        return prefix switch
        {
            "steamid" or "steamids" or "steam" or "steams" => Permission_CheckSteamIds(player, values),
            "flag" or "flags" => Permission_CheckFlags(player, values),
            "group" or "groups" => Permission_CheckGroups(player, values),
            _ => false
        };
    }

    private static bool Permission_CheckSteamIds(CCSPlayerController player, string steamIds)
    {
        if (string.IsNullOrEmpty(steamIds)) return false;

        steamIds = steamIds.Replace("[", "").Replace("]", "");

        var (steam2, steam3, steam32, steam64) = player.SteamID.GetPlayerSteamID();
        var steam3NoBrackets = steam3.Trim('[', ']');

        return steamIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => id.Trim())
            .Any(trimmedId =>
                string.Equals(trimmedId, steam2, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam3NoBrackets, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam32, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam64, StringComparison.OrdinalIgnoreCase)
            );
    }

    private static bool Permission_CheckFlags(CCSPlayerController player, string flags)
    {
        if (player == null || !player.IsValid ||
            player.Connected != PlayerConnectedState.PlayerConnected ||
            player.IsBot || player.IsHLTV)
            return false;

        if (string.IsNullOrEmpty(flags))
            return false;

        var playerData = AdminManager.GetPlayerAdminData(player);
        if (playerData == null)
            return false;

        var requiredFlags = flags
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToList();

        if (playerData._flags != null &&
            requiredFlags.Any(reqFlag =>
                playerData._flags.Contains(reqFlag, StringComparer.OrdinalIgnoreCase)))
            return true;

        var allFlags = playerData.GetAllFlags();
        return allFlags != null &&
            requiredFlags.Any(reqFlag =>
                allFlags.Contains(reqFlag, StringComparer.OrdinalIgnoreCase));
    }

    private static bool Permission_CheckGroups(CCSPlayerController player, string groups)
    {
        if (player == null || !player.IsValid ||
            player.Connected != PlayerConnectedState.PlayerConnected ||
            player.IsBot || player.IsHLTV)
            return false;

        if (string.IsNullOrEmpty(groups))
            return false;

        var playerData = AdminManager.GetPlayerAdminData(player);
        if (playerData == null || playerData.Groups == null || !playerData.Groups.Any())
            return false;

        return groups
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.Trim())
            .Any(reqGroup => playerData.Groups.Contains(reqGroup, StringComparer.OrdinalIgnoreCase));
    }

    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeNone = true, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p =>
                p != null &&
                p.IsValid &&
                p.Connected == PlayerConnectedState.PlayerConnected &&
                (IncludeBots || !p.IsBot) &&
                (IncludeHLTV || !p.IsHLTV) &&
                ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) ||
                (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) ||
                (IncludeNone && p.TeamNum == (byte)CsTeam.None) ||
                (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();
    }
    public static int GetPlayersCount(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities.GetPlayers().Count(p =>
            p != null &&
            p.IsValid &&
            p.Connected == PlayerConnectedState.PlayerConnected &&
            (IncludeBots || !p.IsBot) &&
            (IncludeHLTV || !p.IsHLTV) &&
            ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) ||
            (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) ||
            (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator))
        );
    }

    public static void ClearVariables()
    {
        var g_Main = MainPlugin.Instance.g_Main;

        g_Main.Clear();
    }

    public static void ReloadPlayersGlobals()
    {
        var g_Main = MainPlugin.Instance.g_Main;

        foreach (var players in GetPlayersController(false, false, false))
        {
            if (!players.IsValid()) continue;

            if (g_Main.Player_Data.ContainsKey(players.Slot))
            {
                g_Main.Player_Data.Remove(players.Slot);
            }

            if (g_Main.Player_Disconnect_Reasons.ContainsKey(players.Slot))
            {
                g_Main.Player_Disconnect_Reasons.Remove(players.Slot);
            }

            _ = MainPlugin.Instance.HandlePlayerConnectionsAsync(players, false, "", true);
        }
    }

    public static void CreateResource(string jsonFilePath)
    {
        string headerLine = "////// vvvvvv Add Paths For Precache Resources Down vvvvvvvvvv //////";
        string headerLine2 = "// soundevents/goldkingz_sounds.vsndevts";

        string[] forcedHeaders = { headerLine, headerLine2 };

        if (!File.Exists(jsonFilePath))
        {
            File.WriteAllLines(jsonFilePath, forcedHeaders);
            return;
        }

        var lines = File.ReadAllLines(jsonFilePath).ToList();
        bool needsRewrite = false;

        for (int i = 0; i < forcedHeaders.Length; i++)
        {
            if (lines.Count <= i || lines[i] != forcedHeaders[i])
            {
                needsRewrite = true;
                break;
            }
        }

        if (needsRewrite)
        {
            for (int i = 0; i < Math.Min(forcedHeaders.Length, lines.Count); i++)
            {
                if (forcedHeaders.Contains(lines[i]))
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }

            lines.InsertRange(0, forcedHeaders);
        }

        File.WriteAllLines(jsonFilePath, lines);
    }

    public static string GetGeoIsoCodeInfoAsync(string ipAddress)
    {
        if (!Configs.Instance.AutoSetPlayerLanguage || ipAddress == "127.0.0.1" || ipAddress.Contains("192.168."))
            return "";

        try
        {
            using var reader = new DatabaseReader(Path.GetFullPath(Path.Combine(MainPlugin.Instance.ModuleDirectory, "..", "..", "shared/GoldKingZ/GeoLocation/GeoLite2-City.mmdb")));

            var response = reader.City(ipAddress);

            return response.Country.IsoCode ?? "";
        }
        catch (Exception ex)
        {
            DebugMessage($"GetGeoIsoCodeInfoAsync Error {ex.Message}");
        }
        return "";
    }
    public static void SetPlayerLanguage(CCSPlayerController? player, string isoCode)
    {
        if (!Configs.Instance.AutoSetPlayerLanguage || !player.IsValid()) return;
        if (string.IsNullOrWhiteSpace(isoCode)) return;

        try
        {
            var cultureInfo = CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .FirstOrDefault(c =>
            {
                try
                {
                    var region = new RegionInfo(c.LCID);
                    return region.TwoLetterISORegionName.Equals(isoCode, StringComparison.OrdinalIgnoreCase);
                }
                catch
                {
                    return false;
                }
            });

            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            var steamId = (SteamID)player.SteamID;
            PlayerLanguageManager.Instance.SetLanguage(steamId, cultureInfo);
        }
        catch (Exception ex)
        {
            DebugMessage($"SetPlayerLanguage Error: {ex.Message}");
        }
    }
    
    public static void CheckPlayerInGlobals(CCSPlayerController player)
    {
        var g_Main = MainPlugin.Instance.g_Main;
        if(!player.IsValid() || g_Main.Player_Data.ContainsKey(player.Slot))return;

        var initialData = new Globals.PlayerDataClass(
            player,
            player.SteamID,
            Configs.Instance.CnD_Sounds.CnDSounds == 2 ? 1 : Configs.Instance.CnD_Sounds.CnDSounds == 3 ? 2 : 0,
            Configs.Instance.CnD_Messages.CnDMessages == 2 ? 1 : Configs.Instance.CnD_Messages.CnDMessages == 3 ? 2 : 0,
            DateTime.Now
        );
        g_Main.Player_Data.TryAdd(player.Slot, initialData);
    }

    public static async Task LoadPlayerData(CCSPlayerController player)
    {
        try
        {
            var g_Main = MainPlugin.Instance.g_Main;
            if (!player.IsValid() || g_Main.Player_Data.ContainsKey(player.Slot)) return;

            var steamId = player.SteamID;

            await Server.NextFrameAsync(() =>
            {
                if (!player.IsValid()) return;

                CheckPlayerInGlobals(player);
            });

            if (Configs.Instance.Cookies_Enable > 0)
            {
                try
                {
                    await Server.NextFrameAsync(async () =>
                    {
                        if (!player.IsValid()) return;

                        var cookieData = await Cookies.RetrievePersonDataById(steamId);
                        if (cookieData.PlayerSteamID != 0)
                        {
                            UpdatePlayerData(player, cookieData);
                        }
                    });
                }
                catch (Exception ex)
                {
                    DebugMessage($"LoadPlayerData Update Cookies Error: {ex.Message}");
                }
            }


            if (Configs.Instance.MySql_Enable > 0)
            {
                try
                {
                    var mysqlData = await MySqlDataManager.RetrievePersonDataByIdAsync(steamId);

                    await Server.NextFrameAsync(() =>
                    {
                        if (!player.IsValid()) return;

                        if (mysqlData.PlayerSteamID != 0)
                        {
                            UpdatePlayerData(player, mysqlData);
                        }
                    });
                }
                catch (Exception ex)
                {
                    DebugMessage($"LoadPlayerData Update MySql Error: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"LoadPlayerData Error: {ex.Message}");
        }
    }

    private static void UpdatePlayerData(CCSPlayerController player, Globals_Static.PersonData data)
    {
        if (!player.IsValid()) return;

        var g_Main = MainPlugin.Instance.g_Main;
        if (!g_Main.Player_Data.TryGetValue(player.Slot, out var handle)) return;
        
        if (data.Toggle_Messages < 0 || data.Toggle_Sounds < 0)
        {
            if (data.Toggle_Messages < 0)
            {
                handle.Toggle_Messages = data.Toggle_Messages;
            }

            if (data.Toggle_Sounds < 0)
            {
                handle.Toggle_Sounds = data.Toggle_Sounds;
            }
        }
    }

    public static async Task SavePlayerDataOnDisconnect(CCSPlayerController player)
    {
        try
        {
            if (!player.IsValid()) return;

            var g_Main = MainPlugin.Instance.g_Main;
            var steamId = player.SteamID;

            if (g_Main.Player_Data.TryGetValue(player.Slot, out var alldata))
            {
                if (alldata == null) return;

                if (alldata.Toggle_Messages < 0 || alldata.Toggle_Sounds < 0)
                {
                    var player_SteamID = alldata.SteamId;

                    var player_Toggle_Messages = alldata.Toggle_Messages;
                    var player_Toggle_Sounds = alldata.Toggle_Sounds;

                    if (Configs.Instance.Cookies_Enable == 1)
                    {
                        try
                        {
                            await Server.NextFrameAsync(async () =>
                            {
                                await Cookies.SaveToJsonFile(
                                player_SteamID,
                                player_Toggle_Messages,
                                player_Toggle_Sounds,
                                DateTime.Now
                                );

                            });
                        }
                        catch (Exception ex)
                        {
                            DebugMessage($"SavePlayerDataOnDisconnect Save Cookies Error: {ex.Message}");
                        }
                    }

                    if (Configs.Instance.MySql_Enable == 1)
                    {
                        try
                        {
                            await Server.NextFrameAsync(async () =>
                            {
                                await MySqlDataManager.SaveToMySqlAsync(new Globals_Static.PersonData
                                {
                                    PlayerSteamID = player_SteamID,
                                    Toggle_Messages = player_Toggle_Messages,
                                    Toggle_Sounds = player_Toggle_Sounds,
                                    DateAndTime = DateTime.Now
                                });

                            });
                        }
                        catch (Exception ex)
                        {
                            DebugMessage($"SavePlayerDataOnDisconnect Save MySql Error: {ex.Message}");
                        }
                    }
                }

                g_Main.Player_Data.Remove(player.Slot);
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"SavePlayerDataOnDisconnect Error: {ex.Message}");
        }
    }

    public static void SavePlayersValues()
    {
        var g_Main = MainPlugin.Instance.g_Main;
        foreach (var alldata in g_Main.Player_Data.Values)
        {
            if (alldata == null)
            {
                g_Main.Player_Data.Clear();
                return;
            }

            if (alldata.Toggle_Messages < 0 || alldata.Toggle_Sounds < 0)
            {
                var player_SteamID = alldata.SteamId;

                var player_Toggle_Messages = alldata.Toggle_Messages;
                var player_Toggle_Sounds = alldata.Toggle_Sounds;
                
                if (Configs.Instance.Cookies_Enable == 2)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await Cookies.SaveToJsonFile(
                            player_SteamID,
                            player_Toggle_Messages,
                            player_Toggle_Sounds,
                            DateTime.Now
                            );
                        }
                        catch (Exception ex)
                        {
                            DebugMessage($"SavePlayersValues Save Cookies Error: {ex.Message}");
                        }
                    });
                }


                if (Configs.Instance.MySql_Enable == 2)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await MySqlDataManager.SaveToMySqlAsync(new Globals_Static.PersonData
                            {
                                PlayerSteamID = player_SteamID,
                                Toggle_Messages = player_Toggle_Messages,
                                Toggle_Sounds = player_Toggle_Sounds,
                                DateAndTime = DateTime.Now
                            });
                        }
                        catch (Exception ex)
                        {
                            DebugMessage($"SavePlayersValues Save MySql Error: {ex.Message}");
                        }
                    });
                }
            }
        }

        g_Main.Player_Data.Clear();

        if (Configs.Instance.Cookies_Enable > 0)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await Cookies.RemoveOldEntries();
                }
                catch (Exception ex)
                {
                    DebugMessage($"SavePlayersValues Remove Cookies Error: {ex.Message}");
                }
            });
        }

        if (Configs.Instance.MySql_Enable > 0)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await MySqlDataManager.DeleteOldPlayersAsync();
                }
                catch (Exception ex)
                {
                    DebugMessage($"SavePlayersValues Remove MySql Error: {ex.Message}");
                }
            });
        }
    }

    public static async Task<string> GetPublicIp()
    {
        var services = new[]
        {
            ("https://icanhazip.com", "text"),
            ("https://checkip.amazonaws.com", "text"),
            ("https://api.ipify.org?format=text", "text"),
            ("https://1.1.1.1/cdn-cgi/trace", "cloudflare"),
            ("https://httpbin.org/ip", "httpbin")
        };

        foreach (var (url, type) in services)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                
                var response = await client.GetStringAsync(url);
                string ip = "";

                switch (type)
                {
                    case "text":
                        ip = response.Trim();
                        break;
                        
                    case "cloudflare":
                        ip = response.Split('\n')
                                .FirstOrDefault(line => line.StartsWith("ip="))
                                ?.Split('=')[1]
                                .Trim()!;
                        break;
                        
                    case "httpbin":
                        using (var doc = JsonDocument.Parse(response))
                        {
                            ip = doc.RootElement.GetProperty("origin").GetString()!;
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(ip))
                {
                    var parts = ip.Split('.');
                    if (parts.Length == 4 && parts.All(p => byte.TryParse(p, out _)))
                    {
                        return ip;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugMessage($"Failed to get IP from {url}: {ex.Message}");
            }
        }
        DebugMessage("All IP services failed");
        return "";
    }
    
    public static void DeleteOldFiles(string folderPath, string searchPattern, TimeSpan maxAge)
    {
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            if (directoryInfo.Exists)
            {
                FileInfo[] files = directoryInfo.GetFiles(searchPattern);
                DateTime currentTime = DateTime.Now;
                
                foreach (FileInfo file in files)
                {
                    TimeSpan age = currentTime - file.LastWriteTime;

                    if (age > maxAge)
                    {
                        file.Delete();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"Error in DeleteOldFiles: {ex.Message}");
        }
    }
    
    private static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch (Exception ex)
        {
            DebugMessage(ex.Message);
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }

    public static void MuteCommands(CounterStrikeSharp.API.Modules.UserMessages.UserMessage? um, int Config, bool Fully = false)
    {
        if (um == null) return;
        if (!Fully && Config == 2 || Fully && Config > 0)
        {
            um.Recipients.Clear();
        }
    }
    public static void DebugMessage(string message, bool important = false)
    {
        if (!Configs.Instance.EnableDebug && !important) return;

        Console.ForegroundColor = ConsoleColor.Magenta;
        string output = $"[CnD]: {message}";
        Console.WriteLine(output);

        Console.ResetColor();
    }

    public static async Task<(string Continent, string Country, string CountryCode, string City)> GetGeoInfoAsync(string ipAddress)
    {
        try
        {
            var task = Task.Run(() =>
            {
                using var reader = new DatabaseReader(Path.GetFullPath(Path.Combine(MainPlugin.Instance.ModuleDirectory, "..", "..", "shared/GoldKingZ/GeoLocation/GeoLite2-City.mmdb")));
                {
                    var response = reader.City(ipAddress);
                    return (
                        Continent: response.Continent?.Name ?? MainPlugin.Instance.Localizer["unknown.continent"],
                        Country: response.Country?.Name ?? MainPlugin.Instance.Localizer["unknown.long.country"],
                        CountryCode: response.Country?.IsoCode ?? MainPlugin.Instance.Localizer["unknown.short.country"],
                        City: response.City?.Name ?? MainPlugin.Instance.Localizer["unknown.city"]
                    );
                }
            });

            if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5))) == task)
            {
                return await task;
            }
            
            DebugMessage("GeoIP lookup timed out");
            return (
                MainPlugin.Instance.Localizer["unknown.continent"],
                MainPlugin.Instance.Localizer["unknown.long.country"],
                MainPlugin.Instance.Localizer["unknown.short.country"],
                MainPlugin.Instance.Localizer["unknown.city"]
            );
        }
        catch (Exception ex)
        {
            DebugMessage($"GeoIP lookup error for {ipAddress}: {ex.Message}");
            return (
                MainPlugin.Instance.Localizer["unknown.continent"],
                MainPlugin.Instance.Localizer["unknown.long.country"],
                MainPlugin.Instance.Localizer["unknown.short.country"],
                MainPlugin.Instance.Localizer["unknown.city"]
            );
        }
    }

    public static string GetDisconnectReason(int reasonCode)
    {
        var g_Main = MainPlugin.Instance.g_Main;

        if (g_Main.JsonData_disconnect_reasons == null)
        {
            DebugMessage("config/disconnect_reasons.json not loaded");
            return "config/disconnect_reasons.json not loaded";
        }

        string key = reasonCode.ToString();
        
        return g_Main.JsonData_disconnect_reasons.TryGetValue(key, out JToken? value) 
            ? value.Value<string>() ?? $"Empty message for code {reasonCode}"
            : MainPlugin.Instance.Localizer["unknown.reason"];
    }

    private static readonly HttpClient _httpClient = new HttpClient();
    private static string GetFormattedDateTime() => DateTime.Now.ToString($"{Configs.Instance.Discord_DateFormat} {Configs.Instance.Discord_TimeFormat}");
    public static async Task SendToDiscordAsync(int mode, string webhookUrl, string message, string steamUserId, string steamName, string serverIpPort = null!, bool disconnect = false)
    {
        try
        {
            object payload = mode == 1 
                ? new { content = message }
                : await BuildEmbedPayload(mode, message, steamUserId, steamName, serverIpPort, disconnect);

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(webhookUrl, content);
        }
        catch (Exception ex)
        {
            DebugMessage($"Discord Error: {ex.Message}");
        }
    }

    private static async Task<object> BuildEmbedPayload(int mode, string message, string steamUserId, string steamName, string serverIpPort, bool disconnect = false)
    {
        var color = GetColorFromConfig(disconnect);
        var profileLink = $"https://steamcommunity.com/profiles/{steamUserId}";
        var profilePicture = mode >= 3 ? 
            await GetProfilePictureAsync(steamUserId, Configs.Instance.Discord_UsersWithNoAvatarImage) : null;

        var embed = new
        {
            type = "rich",
            color,
            title = mode == 2 ? steamName : null,
            url = mode == 2 ? profileLink : null,
            description = mode <= 3 ? message : null,
            author = mode >= 3 ? new { name = steamName, url = profileLink, icon_url = profilePicture } : null,
            fields = mode >= 4 ? new[] 
            {
                new { name = "Date/Time", value = GetFormattedDateTime(), inline = false },
                new { name = "Message", value = message, inline = false }
            } : null,
            footer = mode == 5 ? new 
            { 
                text = $"Server IP: {serverIpPort}",
                icon_url = Configs.Instance.Discord_FooterImage 
            } : null
        };

        return new { embeds = new[] { embed } };
    }

    private static int GetColorFromConfig(bool disconnect = false)
    {
        string colorString;
        if(disconnect)
        {
            colorString = Configs.Instance.Discord_Disconnect_SideColor;
        }else
        {
            colorString = Configs.Instance.Discord_Connect_SideColor;
        }

        if (colorString.StartsWith("#"))
        {
            colorString = colorString.Substring(1);
        }
        
        int colorss = int.Parse(colorString, System.Globalization.NumberStyles.HexNumber);
        Color color = Color.FromArgb(colorss >> 16, (colorss >> 8) & 0xFF, colorss & 0xFF);
        return color.ToArgb() & 0xFFFFFF;
    }

    public static async Task<string> GetProfilePictureAsync(string steamId64, string defaultImage)
    {
        try
        {
            string apiUrl = $"https://steamcommunity.com/profiles/{steamId64}/?xml=1";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string xmlResponse = await response.Content.ReadAsStringAsync();
                int startIndex = xmlResponse.IndexOf("<avatarFull><![CDATA[") + "<avatarFull><![CDATA[".Length;
                int endIndex = xmlResponse.IndexOf("]]></avatarFull>", startIndex);

                if (endIndex >= 0)
                {
                    string profilePictureUrl = xmlResponse.Substring(startIndex, endIndex - startIndex);
                    return profilePictureUrl;
                }
                else
                {
                    DebugMessage("Could not find avatarFull tag in XML response, returning default image");
                    return defaultImage;
                }
            }
            else
            {
                DebugMessage($"HTTP request failed with status code: {response.StatusCode}");
                return null!;
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"Error On GetProfilePictureAsync: {ex.Message}");
            return null!;
        }
    }
    
    public static (string, List<string>, string) GetPlayerConnectionSettings(CCSPlayerController p, string type)
    {
        var json = MainPlugin.Instance.g_Main.JsonData_connect_disconnect_config;
        if (json == null) return (null!, new(), null!);

        var group = json.Properties().FirstOrDefault(g => g.Name != "ANY" && IsPlayerInGroupPermission(p, g.Name));
        var target = (group?.Value as JObject) ?? json["ANY"] as JObject;
        if (target == null) return (null!, new(), null!);

        return (
            target.Value<string>($"{type}_MESSAGE_CHAT") ?? "",
            ParseSounds(target[$"{type}_SOUND"]!),
            target.Value<string>($"{type}_SOUND_VOLUME") ?? "100%"
        );
    }

    private static List<string> ParseSounds(JToken token) => token?.Type == JTokenType.Array 
        ? token.ToObject<List<string>>()!.Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
        : token != null ? new() { token.Value<string>()! } : new();

    public static async Task DownloadMissingFilesAsync()
    {
        try
        {
            string Fpath = Path.Combine(MainPlugin.Instance.ModuleDirectory, "GeoLocation");
            if (Directory.Exists(Fpath))
            {
                try
                {
                    Directory.Delete(Fpath, true);
                }
                catch
                {
                    
                }
            }
            
            await DownloadMissingFiles();
            await Server.NextFrameAsync(() => CustomHooks.StartHook());
        }
        catch (Exception ex)
        {
            DebugMessage($"DownloadMissingFiles failed: {ex.Message}");
        }
    }

    public static async Task DownloadMissingFiles()
    {
        try
        {
            string disconnect_reasons = "config/disconnect_reasons.json";
            string disconnect_reasons_GithubUrl = "https://raw.githubusercontent.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/main/Resources/disconnect_reasons.json";
            await DownloadFromGitHub(disconnect_reasons, disconnect_reasons_GithubUrl, Configs.Instance.AutoUpdateDisconnectReasons);

            string connect_disconnect_config = "config/connect_disconnect_config.json";
            string connect_disconnect_config_GithubUrl = "https://raw.githubusercontent.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/main/Resources/connect_disconnect_config.json";
            await DownloadFromGitHub(connect_disconnect_config, connect_disconnect_config_GithubUrl);

            string gamedata = "gamedata/gamedata.json";
            string gamedata_GithubUrl = "https://raw.githubusercontent.com/oqyh/cs2-Private-Plugins/main/Resources/gamedata.json";
            await DownloadFromGitHub(gamedata, gamedata_GithubUrl, Configs.Instance.AutoUpdateSignatures);

            string geoFileName = Path.GetFullPath(Path.Combine(MainPlugin.Instance.ModuleDirectory, "..", "..", "shared/GoldKingZ/GeoLocation/GeoLite2-City.mmdb"));
            string geoUpdateUrl = "https://raw.githubusercontent.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/main/Resources/update.txt";
            await DownloadFromGitHub(geoFileName, geoUpdateUrl, Configs.Instance.AutoUpdateGeoLocation);

        }
        catch (Exception ex)
        {
            DebugMessage($"DownloadMissingFiles Error: {ex.Message}");
        }
    }

    public static async Task DownloadFromGitHub(string filePath, string githubUrl, bool AutoUpdate = false)
    {
        try
        {
            string fullPath = Path.Combine(MainPlugin.Instance.ModuleDirectory, filePath);

            string? dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", $"CS2-Plugin-Connect-Disconnect");
            client.Timeout = TimeSpan.FromSeconds(50);

            string actualDownloadUrl = githubUrl;

            if (githubUrl.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                actualDownloadUrl = await client.GetStringAsync(githubUrl);
                actualDownloadUrl = actualDownloadUrl.Trim();
            }

            byte[] remoteBytes = await client.GetByteArrayAsync(actualDownloadUrl);

            bool needDownload = !File.Exists(fullPath);

            if (!needDownload && AutoUpdate)
            {
                using var sha256 = SHA256.Create();
                string Hash(byte[] b) => BitConverter.ToString(sha256.ComputeHash(b)).Replace("-", "").ToLowerInvariant();

                byte[] localBytes = await File.ReadAllBytesAsync(fullPath);
                needDownload = Hash(localBytes) != Hash(remoteBytes);
            }

            if (needDownload)
            {
                await File.WriteAllBytesAsync(fullPath, remoteBytes);
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"DownloadFromGitHub Error: {ex.Message}");
        }
    }

    public static void RegisterCommandsAndHooks(bool Late_Hook = false)
    {
        Server.ExecuteCommand("sv_hibernate_when_empty false");

        MainPlugin.Instance.RegisterListener<Listeners.OnMapStart>(MainPlugin.Instance.OnMapStart);
        MainPlugin.Instance.RegisterListener<Listeners.OnClientAuthorized>(MainPlugin.Instance.OnClientAuthorized);
        MainPlugin.Instance.RegisterListener<Listeners.OnServerPrecacheResources>(MainPlugin.Instance.OnServerPrecacheResources);
        MainPlugin.Instance.RegisterListener<Listeners.OnMapEnd>(MainPlugin.Instance.OnMapEnd);

        MainPlugin.Instance.RegisterEventHandler<EventPlayerConnectFull>(MainPlugin.Instance.OnEventPlayerConnectFull);
        MainPlugin.Instance.RegisterEventHandler<EventPlayerDeath>(MainPlugin.Instance.OnEventPlayerDeath, HookMode.Pre);
        MainPlugin.Instance.RegisterEventHandler<EventPlayerDisconnect>(MainPlugin.Instance.OnPlayerDisconnect, HookMode.Pre);

        MainPlugin.Instance.AddCommandListener("say", MainPlugin.Instance.OnPlayerSay, HookMode.Post);
        MainPlugin.Instance.AddCommandListener("say_team", MainPlugin.Instance.OnPlayerSay_Team, HookMode.Post);
        MainPlugin.Instance.HookUserMessage(118, MainPlugin.Instance.OnUserMessage_OnSayText2, HookMode.Pre);

        RegisterCssCommands(Configs.Instance.Reload_CnD.Reload_CnD_CommandsInGame.ConvertCommands(), "Commands To Reload Connect Disconnect Sound Plugin", MainPlugin.Instance.Game_UserMessages.CommandsAction_ReloadPlugin);
        RegisterCssCommands(Configs.Instance.CnD_Sounds.CnDSounds_CommandsInGame.ConvertCommands(), "Commands To Toggle Connect Disconnect Sounds", MainPlugin.Instance.Game_UserMessages.CommandsAction_Toggle_Sounds);
        RegisterCssCommands(Configs.Instance.CnD_Messages.CnDMessages_CommandsInGame.ConvertCommands(), "Commands To Toggle Connect Disconnect Messages", MainPlugin.Instance.Game_UserMessages.CommandsAction_Toggle_Messages);
        

        CustomHooks.StartHook();
    }

    public static void RemoveRegisterCommandsAndHooks()
    {
        MainPlugin.Instance.RemoveListener<Listeners.OnMapStart>(MainPlugin.Instance.OnMapStart);
        MainPlugin.Instance.RemoveListener<Listeners.OnClientAuthorized>(MainPlugin.Instance.OnClientAuthorized);
        MainPlugin.Instance.RemoveListener<Listeners.OnServerPrecacheResources>(MainPlugin.Instance.OnServerPrecacheResources);
        MainPlugin.Instance.RemoveListener<Listeners.OnMapEnd>(MainPlugin.Instance.OnMapEnd);

        MainPlugin.Instance.DeregisterEventHandler<EventPlayerConnectFull>(MainPlugin.Instance.OnEventPlayerConnectFull);
        MainPlugin.Instance.DeregisterEventHandler<EventPlayerDeath>(MainPlugin.Instance.OnEventPlayerDeath, HookMode.Pre);
        MainPlugin.Instance.DeregisterEventHandler<EventPlayerDisconnect>(MainPlugin.Instance.OnPlayerDisconnect, HookMode.Pre);

        MainPlugin.Instance.RemoveCommandListener("say", MainPlugin.Instance.OnPlayerSay, HookMode.Post);
        MainPlugin.Instance.RemoveCommandListener("say_team", MainPlugin.Instance.OnPlayerSay_Team, HookMode.Post);
        MainPlugin.Instance.UnhookUserMessage(118, MainPlugin.Instance.OnUserMessage_OnSayText2, HookMode.Pre);

        RemoveCssCommands(Configs.Instance.Reload_CnD.Reload_CnD_CommandsInGame.ConvertCommands(), MainPlugin.Instance.Game_UserMessages.CommandsAction_ReloadPlugin);
        RemoveCssCommands(Configs.Instance.CnD_Sounds.CnDSounds_CommandsInGame.ConvertCommands(), MainPlugin.Instance.Game_UserMessages.CommandsAction_Toggle_Sounds);
        RemoveCssCommands(Configs.Instance.CnD_Messages.CnDMessages_CommandsInGame.ConvertCommands(), MainPlugin.Instance.Game_UserMessages.CommandsAction_Toggle_Messages);

        CustomHooks.UnHook();
    }

    public class MenuEntry
    {
        [String("SteamIDs", "Flags", "Groups")]
        public string Flags { get; set; } = "SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@root,admin | Groups: #css/root,#root,admin";
    }
    public static void LoadJson_connect_disconnect_config(bool playerload = false, CCSPlayerController player = null!, CommandInfo info = null!)
    {
        var g_Main = MainPlugin.Instance.g_Main;
        if (playerload && !player.IsValid()) return;

        string path = Path.Combine(MainPlugin.Instance.ModuleDirectory, "config/connect_disconnect_config.json");

        void Notify(string message, bool successfully = false)
        {
            if (playerload)
            {
                string color = successfully ? "\x06" : "\x02";
                AdvancedPlayerPrintToChat(player, info, $" \x04[CnD]: {color}{message}");
            }
            DebugMessage(message);
        }

        try
        {
            if (!File.Exists(path))
            {
                Notify($"{path} file does not exist.");
                g_Main.JsonData_connect_disconnect_config = null;
                return;
            }

            string[] allLines = File.ReadAllLines(path);
            string jsonContent = string.Join("\n", allLines.Where(l => !l.TrimStart().StartsWith("//")));

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                Notify($"{path} content is empty.");
                g_Main.JsonData_connect_disconnect_config = null;
                return;
            }

            g_Main.JsonData_connect_disconnect_config = JObject.Parse(jsonContent);

            foreach (var property in g_Main.JsonData_connect_disconnect_config.Properties().ToList())
            {
                if (property.Name.Equals("ANY", StringComparison.OrdinalIgnoreCase)) continue;

                var entry = new MenuEntry { Flags = property.Name };
                Configs.ValidateStringRecursive(entry);

                if (entry.Flags != property.Name)
                {
                    var value = g_Main.JsonData_connect_disconnect_config[property.Name];
                    g_Main.JsonData_connect_disconnect_config.Remove(property.Name);
                    g_Main.JsonData_connect_disconnect_config[entry.Flags] = value;
                }
            }

            var formattedJson = g_Main.JsonData_connect_disconnect_config.ToString(Formatting.Indented);
            var commentLines = allLines.Where(l => l.TrimStart().StartsWith("//")).ToList();

            if (commentLines.Count > 0) commentLines.Add("");
            commentLines.Add(formattedJson);

            File.WriteAllText(path, string.Join(Environment.NewLine, commentLines));

            Notify($"{path} Loaded Successfully", true);
        }
        catch (JsonReaderException ex)
        {
            Notify($"JSON Syntax Error in connect_disconnect_config.json: {ex.Message}");
            g_Main.JsonData_connect_disconnect_config = null;
        }
        catch (Exception ex)
        {
            Notify($"General Error loading connect_disconnect_config.json: {ex.Message}");
            g_Main.JsonData_connect_disconnect_config = null;
        }
    }

    public static void LoadJson_disconnect_reasons(bool playerload = false, CCSPlayerController player = null!, CommandInfo info = null!)
    {
        var g_Main = MainPlugin.Instance.g_Main;
        if (playerload && !player.IsValid()) return;

        string path = Path.Combine(MainPlugin.Instance.ModuleDirectory, "config/disconnect_reasons.json");

        void Notify(string message, bool successfully = false)
        {
            if (playerload)
            {
                string color = successfully ? "\x06" : "\x02";
                AdvancedPlayerPrintToChat(player, info, $" \x04[CnD]: {color}{message}");
            }
            DebugMessage(message);
        }

        try
        {
            if (!File.Exists(path))
            {
                Notify($"{path} file does not exist.");
                g_Main.JsonData_disconnect_reasons = null;
                return;
            }

            string[] allLines = File.ReadAllLines(path);
            string jsonContent = string.Join("\n", allLines.Where(l => !l.TrimStart().StartsWith("//")));

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                Notify($"{path} content is empty.");
                g_Main.JsonData_disconnect_reasons = null;
                return;
            }

            g_Main.JsonData_disconnect_reasons = JObject.Parse(jsonContent);
            var formattedJson = g_Main.JsonData_disconnect_reasons.ToString(Formatting.Indented);
            var commentLines = allLines.Where(l => l.TrimStart().StartsWith("//")).ToList();

            if (commentLines.Count > 0) commentLines.Add("");
            commentLines.Add(formattedJson);

            File.WriteAllText(path, string.Join(Environment.NewLine, commentLines));

            Notify($"{path} Loaded Successfully", true);
        }
        catch (JsonReaderException ex)
        {
            Notify($"JSON Syntax Error in disconnect_reasons.json: {ex.Message}");
            g_Main.JsonData_disconnect_reasons = null;
        }
        catch (Exception ex)
        {
            Notify($"General Error loading disconnect_reasons.json: {ex.Message}");
            g_Main.JsonData_disconnect_reasons = null;
        }
    }
}