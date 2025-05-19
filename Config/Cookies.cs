using Newtonsoft.Json;
using CnD_Sound.Config;
using CounterStrikeSharp.API;
using System.Collections.Concurrent;

namespace CnD_Sound;

public class Cookies
{
    public static void SaveToJsonFile(ulong PlayerSteamID, int Toggle_Messages, int Toggle_Sounds, DateTime DateAndTime)
    {
        if (!Configs.GetConfigData().Cookies_Enable) return;

        const string fileName = "cookies.json";
        var cookiesPath = Path.Combine(MainPlugin.Instance.ModuleDirectory, "cookies", fileName);

        try
        {
            var data = new ConcurrentDictionary<ulong, Globals_Static.PersonData>();
            
            if (File.Exists(cookiesPath))
            {
                var json = File.ReadAllText(cookiesPath);
                var existing = JsonConvert.DeserializeObject<List<Globals_Static.PersonData>>(json);
                existing?.ForEach(p => data.TryAdd(p.PlayerSteamID, p));
            }

            data.AddOrUpdate(PlayerSteamID, 
                new Globals_Static.PersonData
                {
                    PlayerSteamID = PlayerSteamID,
                    Toggle_Messages = Toggle_Messages,
                    Toggle_Sounds = Toggle_Sounds,
                    DateAndTime = DateAndTime
                },
                (_, existing) => new Globals_Static.PersonData
                {
                    PlayerSteamID = PlayerSteamID,
                    Toggle_Messages = Toggle_Messages,
                    Toggle_Sounds = Toggle_Sounds,
                    DateAndTime = DateAndTime
                });

            _ = Task.Run(() => 
            {
                try 
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(cookiesPath)!);
                    File.WriteAllText(cookiesPath, JsonConvert.SerializeObject(data.Values.ToList(), Formatting.Indented));
                }
                catch (Exception ex)
                {
                    Helper.DebugMessage($"File write error: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"SaveToJsonFile Critical error: {ex.Message}");
        }
    }

    public static Globals_Static.PersonData RetrievePersonDataById(ulong targetId)
    {
        string cookiesDirectory = Path.Combine(MainPlugin.Instance.ModuleDirectory, "cookies");
        string cookiesFilePath = Path.Combine(cookiesDirectory, "cookies.json");
        try
        {
            if (Directory.Exists(cookiesDirectory) && File.Exists(cookiesFilePath))
            {
                string jsonData = File.ReadAllText(cookiesFilePath);
                List<Globals_Static.PersonData> allPersonsData = JsonConvert.DeserializeObject<List<Globals_Static.PersonData>>(jsonData) ?? new List<Globals_Static.PersonData>();

                Globals_Static.PersonData targetPerson = allPersonsData.Find(p => p.PlayerSteamID == targetId)!;

                if (targetPerson != null)
                {
                    return targetPerson;
                }
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage(ex.Message);
        }
        return new Globals_Static.PersonData();
    }

    public static void FetchAndRemoveOldJsonEntries()
    {
        if(Configs.GetConfigData().Cookies_AutoRemovePlayerOlderThanXDays < 1) return;

        string cookiesDirectory = Path.Combine(MainPlugin.Instance.ModuleDirectory, "cookies");
        string cookiesFilePath = Path.Combine(cookiesDirectory, "cookies.json");
        try
        {
            if (Directory.Exists(cookiesDirectory) && File.Exists(cookiesFilePath))
            {
                string jsonData = File.ReadAllText(cookiesFilePath);
                List<Globals_Static.PersonData> allPersonsData = JsonConvert.DeserializeObject<List<Globals_Static.PersonData>>(jsonData) ?? new List<Globals_Static.PersonData>();

                int daysToKeep = Configs.GetConfigData().Cookies_AutoRemovePlayerOlderThanXDays;
                allPersonsData.RemoveAll(p => (DateTime.Now - p.DateAndTime).TotalDays > daysToKeep);

                string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                try
                {
                    File.WriteAllText(cookiesFilePath, updatedJsonData);
                }
                catch (Exception ex)
                {
                    Helper.DebugMessage(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage(ex.Message);
        }
    }
}