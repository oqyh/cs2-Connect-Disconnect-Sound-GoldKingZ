using CounterStrikeSharp.API.Core;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Timers;
using System.Globalization;
using Newtonsoft.Json.Converters;
using System.Drawing;
using CounterStrikeSharp.API.Modules.UserMessages;
using System.Security.Cryptography;
using CounterStrikeSharp.API.Modules.Cvars;


namespace CnD_Sound;

public static class Extension
{
    public static bool IsValid([NotNullWhen(true)] this CCSPlayerController? player, bool IncludeBots = false, bool IncludeHLTV = false)
    {
        if (player == null || !player.IsValid)
            return false;

        if (!IncludeBots && player.IsBot)
            return false;

        if (!IncludeHLTV && player.IsHLTV)
            return false;

        return true;
    }

    public static bool HasValidPermissionData(this string? groups)
    {
        if (string.IsNullOrWhiteSpace(groups)) return false;

        var segments = groups.Split('|', StringSplitOptions.RemoveEmptyEntries);
        foreach (var seg in segments)
        {
            var trimmed = seg.Trim();
            if (string.IsNullOrEmpty(trimmed))
                continue;

            int colonIndex = trimmed.IndexOf(':');
            if (colonIndex == -1 || colonIndex == 0)
                continue;

            string values = trimmed.Substring(colonIndex + 1).Trim();
            if (!string.IsNullOrEmpty(values))
                return true;
        }

        return false;
    }
    
    private static readonly HashSet<string> _colorNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Default", "White", "DarkRed", "Green", "LightYellow", "LightBlue",
        "Olive", "Lime", "Red", "LightPurple", "Purple", "Grey", "Yellow",
        "Gold", "Silver", "Blue", "DarkBlue", "BlueGrey", "Magenta",
        "LightRed", "Orange", "Darkred"
    };

    public static string RemoveColorNames(this string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        string pattern = @"\{([^}]+)\}";
        return System.Text.RegularExpressions.Regex.Replace(input, pattern, match =>
        {
            string colorName = match.Groups[1].Value;
            return _colorNames.Contains(colorName) ? "" : match.Value;
        });
    }
    public static string ReplaceMessages(this string MessageFormat,string date = "",string time = "",string PlayerName = "",
    string SteamId = "",string SteamId3 = "",string SteamId32 = "",string SteamId64 = "",string ipAddress = "",
    string Continent = "",string Country = "",string SCountry = "",string City = "",string reason = "")
    {
        return MessageFormat?
            .ReplaceIgnoreCase("{DATE}", date)
            .ReplaceIgnoreCase("{TIME}", time)
            .ReplaceIgnoreCase("{PLAYERNAME}", PlayerName)
            .ReplaceIgnoreCase("{STEAMID}", SteamId)
            .ReplaceIgnoreCase("{STEAMID3}", SteamId3)
            .ReplaceIgnoreCase("{STEAMID32}", SteamId32)
            .ReplaceIgnoreCase("{STEAMID64}", SteamId64)
            .ReplaceIgnoreCase("{IP}", ipAddress)
            .ReplaceIgnoreCase("{CONTINENT}", Continent)
            .ReplaceIgnoreCase("{LONGCOUNTRY}", Country)
            .ReplaceIgnoreCase("{SHORTCOUNTRY}", SCountry)
            .ReplaceIgnoreCase("{CITY}", City)
            .ReplaceIgnoreCase("{DISCONNECT_REASON}", reason)
            ?? string.Empty;
    }

public static string ReplaceIgnoreCase(this string source, string oldValue, string newValue)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(oldValue))
            return source;
            
        return source.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);
    }

    public static int ToggleOnOff(this int value)
    {
        return value switch
        {
            1 => -2,
            2 => -1,
            -1 => -2,
            -2 => -1,
            _ => value
        };
    }

    public static float ToPercentageFloat(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return 1f;
        }

        input = input.Replace("%", "").Trim();

        if (!float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            return 1f;
        }

        return Math.Clamp(result / 100f, 0f, 1f);
    }

    public static string GetSoundPath(this List<string> sounds, bool PickSoundsByOrder)
    {
        if (sounds == null || sounds.Count == 0)return string.Empty;

        var validSounds = sounds.Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => s.Trim())
                            .ToList();

        if (validSounds.Count == 0)return string.Empty;

        if (validSounds.Count == 1)return validSounds[0];

        string groupKey = string.Join("|", validSounds);
        var tracker = MainPlugin.Instance.g_Main.GlobalSoundTracker;

        if (!tracker.TryGetValue(groupKey, out HashSet<string>? used))
        {
            used = new HashSet<string>();
            tracker[groupKey] = used;
        }

        if (used.Count >= validSounds.Count)
        {
            used.Clear();
        }
        
        string selected;

        if (PickSoundsByOrder)
        {
            selected = validSounds.First(s => !used.Contains(s));
        }
        else
        {
            var available = validSounds.Where(s => !used.Contains(s)).ToList();
            selected = available[GetRandomInt(0, available.Count)];
        }

        used.Add(selected);
        return selected;
    }

    private static int GetRandomInt(int minValue, int maxValue)
    {
        if (minValue >= maxValue)
        {
            return minValue;
        }
            

        byte[] randomNumber = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        int range = maxValue - minValue;
        int absoluteValue = Math.Abs(BitConverter.ToInt32(randomNumber, 0));
        return minValue + (absoluteValue % range);
    }
    
    

    private const ulong Steam64Offset = 76561197960265728UL;
    public static (string steam2, string steam3, string steam32, string steam64) GetPlayerSteamID(this ulong steamId64)
    {
        uint id32 = (uint)(steamId64 - Steam64Offset);
        var steam32 = id32.ToString();
        uint y = id32 & 1;
        uint z = id32 >> 1;
        var steam2 = $"STEAM_0:{y}:{z}";
        var steam3 = $"[U:1:{steam32}]";
        var steam64 = steamId64.ToString();
        return (steam2, steam3, steam32, steam64);
    }

    public static string[]? ConvertCommands(this string input, bool EventPlayerChat = false)
    {
        var parts = input.Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split(':', 2))
            .ToDictionary(
                p => p[0].Trim(),
                p => p.Length > 1
                    ? p[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(c => c.Trim())
                        .Where(c => !string.IsNullOrEmpty(c))
                    : Enumerable.Empty<string>()
            );

        if (!parts.Values.Any(v => v.Any())) return null;

        if (!EventPlayerChat)
        {
            return parts.FirstOrDefault().Value?.Select(c =>
            {
                if (c.StartsWith("!"))
                {
                    var cmd = c.TrimStart('!');
                    return cmd.StartsWith("css_") ? cmd : "css_" + cmd;
                }
                return c;
            }).Distinct().ToArray();
        }

        var first = parts.FirstOrDefault().Value?
            .Select(c =>
            {
                var cmd = c.TrimStart('!');
                if (cmd.StartsWith("css_"))
                    cmd = cmd.Substring(4);
                return "!" + cmd;
            }) ?? Enumerable.Empty<string>();

        var rest = parts.Skip(1).SelectMany(p => p.Value);
        var result = first.Concat(rest).Distinct().ToArray();

        return result.Length == 0 ? null : result;
    }
}