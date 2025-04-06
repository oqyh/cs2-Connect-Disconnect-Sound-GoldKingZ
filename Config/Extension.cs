using CounterStrikeSharp.API.Core;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;


namespace CnD_Sound;

public static class Extension
{
    public static bool IsValid([NotNullWhen(true)] this CCSPlayerController? player, bool IncludeBots = false, bool IncludeHLTV  = false)
    {
        if (player == null || !player.IsValid)
            return false;

        if (!IncludeBots && player.IsBot)
            return false;

        if (!IncludeHLTV && player.IsHLTV)
            return false;

        return true;
    }


    public static string ReplaceMessages(this string MessageFormate, string date, string time, string PlayerName, string SteamId, string SteamId3, string SteamId32, string SteamId64, string ipAddress, string Continent, string Country, string SCountry, string City, string reason)
    {
        var replacedMessage = MessageFormate
                            .Replace("{DATE}", date.ToString())
                            .Replace("{TIME}", time.ToString())
                            .Replace("{PLAYERNAME}", PlayerName.ToString())
                            .Replace("{STEAMID}", SteamId.ToString())
                            .Replace("{STEAMID3}", SteamId3.ToString())
                            .Replace("{STEAMID32}", SteamId32.ToString())
                            .Replace("{STEAMID64}", SteamId64.ToString())
                            .Replace("{IP}", ipAddress.ToString())
                            .Replace("{CONTINENT}", Continent.ToString())
                            .Replace("{LONGCOUNTRY}", Country.ToString())
                            .Replace("{SHORTCOUNTRY}", SCountry.ToString())
                            .Replace("{CITY}", City.ToString())
                            .Replace("{DISCONNECT_REASON}", reason.ToString());
        return replacedMessage;
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
            return 0.5f;
        }

        input = input.Replace("%", "").Trim();

        if (float.TryParse(input, out float result))
        {
            return result / 100f;
        }
        
        return 0.5f;
    }

    private static readonly Random rng = new Random();
    public static string GetNextSound(this List<string> sounds, bool pickRandom)
    {
        if (sounds == null || sounds.Count == 0) return null!;
        var g_Main = MainPlugin.Instance.g_Main;

        var key = $"{string.Join("|", sounds)}|{pickRandom}";

        if (pickRandom)
        {
            if (!g_Main.randomQueues.TryGetValue(key, out var queue) || queue.Count == 0)
            {
                var shuffled = sounds.OrderBy(_ => rng.Next()).ToList();
                queue = new Queue<string>(shuffled);
                g_Main.randomQueues[key] = queue;
            }
            
            return queue.Dequeue();
        }
        else
        {
            if (!g_Main.sequentialIndices.TryGetValue(key, out int index))
            {
                index = 0;
                g_Main.sequentialIndices[key] = index;
            }

            var result = sounds[index];
            g_Main.sequentialIndices[key] = (index + 1) % sounds.Count;
            return result;
        }
    }
}