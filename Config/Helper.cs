using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text.Json;
using CnD_Sound.Config;
using System.Text.Encodings.Web;
using System.Text;
using System.Drawing;
using CounterStrikeSharp.API.Modules.Cvars;
using System.Runtime.InteropServices;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CounterStrikeSharp.API.Core.Translations;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using System.Security.Cryptography;


namespace CnD_Sound;

public class Helper
{

    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message))return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                player.PrintToChat(" " + messages);
            }
        }else
        {
            player.PrintToChat(message);
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
        if (string.IsNullOrEmpty(groups))
        {
            return false;
        }
        var Groups = groups.Split(',');
        foreach (var group in Groups)
        {
            if (string.IsNullOrEmpty(group))
            {
                continue;
            }
            string groupId = group[0] == '!' ? group.Substring(1) : group;
            if (group[0] == '#' && AdminManager.PlayerInGroup(player, group))
            {
                return true;
            }
            else if (group[0] == '@' && AdminManager.PlayerHasPermissions(player, group))
            {
                return true;
            }
            else if (group[0] == '!' && player.AuthorizedSteamID != null && (groupId == player.AuthorizedSteamID.SteamId2.ToString() || groupId == player.AuthorizedSteamID.SteamId3.ToString().Trim('[', ']') ||
            groupId == player.AuthorizedSteamID.SteamId32.ToString() || groupId == player.AuthorizedSteamID.SteamId64.ToString()))
            {
                return true;
            }
            else if (AdminManager.PlayerInGroup(player, group))
            {
                return true;
            }
        }
        return false;
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
        
        SavePlayersValues();
        g_Main.OnLoop.Clear();
        g_Main.randomQueues.Clear();
        g_Main.sequentialIndices.Clear();
        g_Main.JsonData_Disconnect = null;
        g_Main.JsonData_Settings = null;
    }

    public static void CreateResource(string jsonFilePath)
    {
        string headerLine = "////// vvvvvv Add Paths For Precache Resources Down vvvvvvvvvv //////";
        string headerLine2 = "soundevents/goldkingz_sounds.vsndevts";
        string headerLine3 = "soundevents/addons_goldkingz_sounds.vsndevts";
        if (!File.Exists(jsonFilePath))
        {
            using (StreamWriter sw = File.CreateText(jsonFilePath))
            {
                sw.WriteLine(headerLine);
                sw.WriteLine(headerLine2);
                sw.WriteLine(headerLine3);
            }
        }
        else
        {
            string[] lines = File.ReadAllLines(jsonFilePath);
            if (lines.Length == 0 || lines[0] != headerLine)
            {
                using (StreamWriter sw = new StreamWriter(jsonFilePath))
                {
                    sw.WriteLine(headerLine);
                    foreach (string line in lines)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }
    }

    public static async Task LoadPlayerData(CCSPlayerController player)
    {
        try
        {
            var g_Main = MainPlugin.Instance.g_Main;
            if (!player.IsValid() || g_Main.Player_Data.ContainsKey(player)) return;
            
            var steamId = player.SteamID;

            await Server.NextFrameAsync(() => 
            {
                if (!player.IsValid() || g_Main.Player_Data.ContainsKey(player)) return;

                var initialData = new Globals.PlayerDataClass(
                    player,
                    steamId,
                    false,
                    Configs.GetConfigData().Default_Messages ? 1 : 2, 
                    Configs.GetConfigData().Default_Sounds ? 1 : 2
                );
                g_Main.Player_Data.TryAdd(player, initialData);
            });

            if (Configs.GetConfigData().Cookies_Enable)
            {
                await Server.NextFrameAsync(() => 
                {
                    if (!player.IsValid()) return;
                    
                    var cookieData = Cookies.RetrievePersonDataById(steamId);

                    if (cookieData.PlayerSteamID != 0)
                    {
                        UpdatePlayerData(player, cookieData);
                    }
                });
            }

            if (Configs.GetConfigData().MySql_Enable)
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
                    DebugMessage($"Error in MySql LoadPlayerData: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"LoadPlayerData error: {ex.Message}");
        }
    }

    private static void UpdatePlayerData(CCSPlayerController player, Globals_Static.PersonData data)
    {
        if (!player.IsValid() || !MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player, out var handle))return;

        if(data.Toggle_Messages < 0 || data.Toggle_Sounds < 0)
        {
            if(data.Toggle_Messages < 0)
            {
                handle.Toggle_Messages = data.Toggle_Messages;
            }

            if(data.Toggle_Sounds < 0)
            {
                handle.Toggle_Sounds = data.Toggle_Sounds;
            }

            Cookies.SaveToJsonFile(
                handle.SteamId,
                handle.Toggle_Messages,
                handle.Toggle_Sounds,
                DateTime.Now
            );
        }
    }

    public static void SavePlayersValues()
    {
        var g_Main = MainPlugin.Instance.g_Main;
        foreach(var alldata in g_Main.Player_Data.Values)
        {
            if(alldata == null)
            {
                g_Main.Player_Data.Clear();
                return;
            }

            if(alldata.Toggle_Messages < 0 || alldata.Toggle_Sounds < 0)
            {
                var player_SteamID = alldata.SteamId;
                var player_Toggle_Messages = alldata.Toggle_Messages;
                var player_Toggle_Sounds = alldata.Toggle_Sounds;

                Cookies.SaveToJsonFile(player_SteamID,
                    player_Toggle_Messages,
                    player_Toggle_Sounds,
                    DateTime.Now
                );

                if (Configs.GetConfigData().MySql_Enable)
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
                            DebugMessage($"SavePlayerValues error: {ex.Message}");
                        }
                    });
                }
            }
        }

        g_Main.Player_Data.Clear();

        if (Configs.GetConfigData().Cookies_Enable)
        {
            try 
            {
                Cookies.FetchAndRemoveOldJsonEntries();
            }
            catch (Exception ex)
            {
                DebugMessage($"Cookie cleanup error: {ex.Message}");
            }
        }

        if (Configs.GetConfigData().MySql_Enable)
        {
            _ = Task.Run(async () =>
            {
                try
                {                        
                    await MySqlDataManager.DeleteOldPlayersAsync();
                }
                catch (Exception ex)
                {
                    DebugMessage($"MySQL cleanup error: {ex.Message}");
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

    public static void DebugMessage(string message, bool prefix = true)
    {
        if (!Configs.GetConfigData().EnableDebug) return;

        Console.ForegroundColor = ConsoleColor.Magenta;
        string output = prefix ? $"[CnD]: {message}" : message;
        Console.WriteLine(output);
        
        Console.ResetColor();
    }

    public static async Task<(string Continent, string Country, string CountryCode, string City)> GetGeoInfoAsync(string ipAddress)
    {
        try
        {
            var task = Task.Run(() =>
            {
                using (var reader = new DatabaseReader(Path.Combine(MainPlugin.Instance.ModuleDirectory, "GeoLocation/GeoLite2-City.mmdb")))
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


    public static void LoadJson()
    {
        var g_Main = MainPlugin.Instance.g_Main;

        try
        {
            string disconnectJsonPath = Path.Combine(MainPlugin.Instance.ModuleDirectory, "config/disconnect_reasons.json");
            if (!File.Exists(disconnectJsonPath))
            {
                DebugMessage($"{disconnectJsonPath} file does not exist.");
                g_Main.JsonData_Disconnect = null;
            }
            else
            {
                string jsonContent = File.ReadAllText(disconnectJsonPath);
                if (string.IsNullOrEmpty(jsonContent))
                {
                    DebugMessage($"{disconnectJsonPath} content is empty.");
                    g_Main.JsonData_Disconnect = null;
                }
                else
                {
                    g_Main.JsonData_Disconnect = JObject.Parse(jsonContent);
                    DebugMessage($"{disconnectJsonPath} Loaded Successfully");
                }
            }
        }
        catch (JsonReaderException ex)
        {
            DebugMessage($"JSON Syntax Error in disconnect_reasons.json: {ex.Message}");
            g_Main.JsonData_Disconnect = null;
        }
        catch (Exception ex)
        {
            DebugMessage($"General Error loading disconnect_reasons.json: {ex.Message}");
            g_Main.JsonData_Disconnect = null;
        }

        try
        {
            string settingsJsonPath = Path.Combine(MainPlugin.Instance.ModuleDirectory, "config/connect_disconnect_config.json");
            if (!File.Exists(settingsJsonPath))
            {
                DebugMessage($"{settingsJsonPath} file does not exist.");
                g_Main.JsonData_Settings = null;
            }
            else
            {
                string jsonContent = File.ReadAllText(settingsJsonPath);
                if (string.IsNullOrEmpty(jsonContent))
                {
                    DebugMessage($"{settingsJsonPath} content is empty.");
                    g_Main.JsonData_Settings = null;
                }
                else
                {
                    g_Main.JsonData_Settings = JObject.Parse(jsonContent);
                    DebugMessage($"{settingsJsonPath} Loaded Successfully");
                }
            }
        }
        catch (JsonReaderException ex)
        {
            DebugMessage($"JSON Syntax Error in connect_disconnect_config.json: {ex.Message}");
            g_Main.JsonData_Settings = null;
        }
        catch (Exception ex)
        {
            DebugMessage($"General Error loading connect_disconnect_config.json: {ex.Message}");
            g_Main.JsonData_Settings = null;
        }
    }

    public static string GetDisconnectReason(int reasonCode)
    {
        var g_Main = MainPlugin.Instance.g_Main;

        if (g_Main.JsonData_Disconnect == null)
        {
            Helper.DebugMessage("config/disconnect_reasons.json not loaded");
            return "config/disconnect_reasons.json not loaded";
        }

        string key = reasonCode.ToString();
        
        return g_Main.JsonData_Disconnect.TryGetValue(key, out JToken? value) 
            ? value.Value<string>() ?? $"Empty message for code {reasonCode}"
            : MainPlugin.Instance.Localizer["unknown.reason"];
    }

    private const string UpdateFileUrl = "https://raw.githubusercontent.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/main/Resources/update.txt";
    public static async Task<bool> CheckAndUpdateGeoAsync(string localFilePath = null!)
    {
        localFilePath ??= Path.Combine(
            MainPlugin.Instance.ModuleDirectory, 
            "GeoLocation", 
            "GeoLite2-City.mmdb"
        );

        using (var httpClient = new HttpClient())
        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20)))
        {
            try
            {
                var directory = Path.GetDirectoryName(localFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                bool autoUpdate = Configs.GetConfigData().AutoUpdateGeoLocation;
                bool fileExists = File.Exists(localFilePath);

                if (fileExists && !autoUpdate)
                {
                    return false;
                }

                string downloadUrl = await httpClient.GetStringAsync(UpdateFileUrl, cts.Token);
                downloadUrl = downloadUrl.Trim();

                byte[] remoteData = await httpClient.GetByteArrayAsync(downloadUrl, cts.Token);
                string remoteHash = ComputeSha256Hash(remoteData);

                if (!fileExists)
                {
                    await File.WriteAllBytesAsync(localFilePath, remoteData, cts.Token);
                    return true;
                }
                else
                {
                    byte[] localData = await File.ReadAllBytesAsync(localFilePath);
                    string localHash = ComputeSha256Hash(localData);
                    
                    if (localHash != remoteHash)
                    {
                        await File.WriteAllBytesAsync(localFilePath, remoteData, cts.Token);
                        return true;
                    }
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                DebugMessage("CheckAndUpdateGeoAsync Timed Out After 20 Seconds");
                return false;
            }
            catch (Exception ex)
            {
                DebugMessage($"Error On CheckAndUpdateDatabaseAsync: {ex.Message}");
                return false;
            }
        }
    }

    private static string ComputeSha256Hash(byte[] data)
    {
        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(data);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }


    private static readonly HttpClient _httpClient = new HttpClient();
    private static string GetFormattedDateTime() => DateTime.Now.ToString($"{Configs.GetConfigData().Discord_DateFormat} {Configs.GetConfigData().Discord_TimeFormat}");
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
            await GetProfilePictureAsync(steamUserId, Configs.GetConfigData().Discord_UsersWithNoAvatarImage) : null;

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
                icon_url = Configs.GetConfigData().Discord_FooterImage 
            } : null
        };

        return new { embeds = new[] { embed } };
    }

    private static int GetColorFromConfig(bool disconnect = false)
    {
        string colorString;
        if(disconnect)
        {
            colorString = Configs.GetConfigData().Discord_Disconnect_SideColor;
        }else
        {
            colorString = Configs.GetConfigData().Discord_Connect_SideColor;
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

    public static async Task DownloadMissingFiles()
    {
        try
        {
            string baseFolderPath = MainPlugin.Instance.ModuleDirectory;

            string ReasonsConfigFileName = "config/disconnect_reasons.json";
            string ReasonsConfigGithubUrl = "https://raw.githubusercontent.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/main/Resources/disconnect_reasons.json";
            string ReasonsConfigFilePath = Path.Combine(baseFolderPath, ReasonsConfigFileName);
            string ReasonsConfigDirectoryPath = Path.GetDirectoryName(ReasonsConfigFilePath)!;
            await DownloadFileIfNotExists(ReasonsConfigFilePath, ReasonsConfigGithubUrl, ReasonsConfigDirectoryPath);
            
            string CnDConfigFileName = "config/connect_disconnect_config.json";
            string CnDConfigGithubUrl = "https://raw.githubusercontent.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/main/Resources/connect_disconnect_config.json";
            string CnDConfigFilePath = Path.Combine(baseFolderPath, CnDConfigFileName);
            string CnDConfigDirectoryPath = Path.GetDirectoryName(CnDConfigFilePath)!;
            await DownloadFileIfNotExists(CnDConfigFilePath, CnDConfigGithubUrl, CnDConfigDirectoryPath);

            string gamedataFileName = "gamedata/gamedata.json";
            string gamedataGithubUrl = "https://raw.githubusercontent.com/oqyh/cs2-Private-Plugins/main/Resources/gamedata.json";
            string gamedataFilePath = Path.Combine(baseFolderPath, gamedataFileName);
            string gamedataDirectoryPath = Path.GetDirectoryName(gamedataFilePath)!;
            await CheckAndDownloadFile(gamedataFilePath, gamedataGithubUrl, gamedataDirectoryPath);
        }
        catch (Exception ex)
        {
            DebugMessage($"Error in DownloadMissingFiles: {ex.Message}");
        }
    }

    public static async Task<bool> CheckAndDownloadFile(string filePath, string githubUrl, string directoryPath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                await DownloadFileFromGithub(githubUrl, filePath);
                return true;
            }
            else
            {
                if (Configs.GetConfigData().AutoUpdateSignatures)
                {
                    bool isFileDifferent = await IsFileDifferent(filePath, githubUrl);
                    if (isFileDifferent)
                    {
                        File.Delete(filePath);
                        await DownloadFileFromGithub(githubUrl, filePath);
                        return true;
                    }
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            DebugMessage($"Error in CheckAndDownloadFile: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> IsFileDifferent(string localFilePath, string githubUrl)
    {
        try
        {
            byte[] localFileBytes = await File.ReadAllBytesAsync(localFilePath);
            string localFileHash = GetFileHash(localFileBytes);

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    byte[] githubFileBytes = await client.GetByteArrayAsync(githubUrl, cts.Token);
                    string githubFileHash = GetFileHash(githubFileBytes);
                    return localFileHash != githubFileHash;
                }
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"Error comparing files: {ex.Message}");
            return false;
        }
    }

    public static string GetFileHash(byte[] fileBytes)
    {
        try
        {
            using (var md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(fileBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"Error generating file hash: {ex.Message}");
            return string.Empty;
        }
    }

    public static async Task DownloadFileIfNotExists(string filePath, string githubUrl, string directoryPath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                await DownloadFileFromGithub(githubUrl, filePath);
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"Error in DownloadFileIfNotExists: {ex.Message}");
        }
    }

    public static async Task DownloadFileFromGithub(string url, string destinationPath)
    {
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            
            try
            {
                byte[] fileBytes = await client.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(destinationPath, fileBytes);
            }
            catch (Exception ex)
            {
                DebugMessage($"Error downloading file: {ex.Message}");
            }
        }
    }
    public static (string, List<string>, string) GetPlayerConnectionSettings(CCSPlayerController p, string type)
    {
        var json = MainPlugin.Instance.g_Main.JsonData_Settings;
        if (json == null) return (null!, new(), null!);

        var group = json.Properties().FirstOrDefault(g => g.Name != "ANY" && IsPlayerInGroupPermission(p, g.Name));
        var target = (group?.Value as JObject) ?? json["ANY"] as JObject;
        if (target == null) return (null!, new(), null!);

        return (
            target.Value<string>($"{type}_MESSAGE_CHAT") ?? "",
            ParseSounds(target[$"{type}_SOUND"]!),
            target.Value<string>($"{type}_SOUND_VOLUME") ?? ""
        );
    }

    private static List<string> ParseSounds(JToken token) => token?.Type == JTokenType.Array 
        ? token.ToObject<List<string>>()!.Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
        : token != null ? new() { token.Value<string>()! } : new();
}