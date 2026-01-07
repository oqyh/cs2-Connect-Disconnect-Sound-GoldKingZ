using SteamDatabase.ValvePak;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace CnD_Sound;

public class AddonManagerConfig
{
    private static readonly string StartDirectory = MainPlugin.Instance.ModuleDirectory;
    
    public static List<string> GetWorkshopIdsFromConfig()
    {
        var workshopIds = new List<string>();
        
        try
        {
            string configPath = GetConfigFilePath();
            if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
            {
                return workshopIds;
            }

            string configContent = File.ReadAllText(configPath);
            workshopIds = ExtractWorkshopIds(configContent);
        }
        catch (Exception ex)
        {
            Helper.DebugMessage("Error reading workshop IDs from config: " + ex.Message);
        }
        
        return workshopIds;
    }

    private static string GetConfigFilePath()
    {
        try
        {
            DirectoryInfo moduleDir = new DirectoryInfo(StartDirectory);
            DirectoryInfo gameDir = moduleDir.Parent?.Parent?.Parent?.Parent!;
            
            if (gameDir == null || !gameDir.Exists)
                return null!;

            string configPath = Path.Combine(
                gameDir.FullName,
                "cfg", "multiaddonmanager", "multiaddonmanager.cfg"
            );

            return File.Exists(configPath) ? configPath : null!;
        }
        catch
        {
            return null!;
        }
    }

    private static List<string> ExtractWorkshopIds(string configContent)
    {
        var workshopIds = new List<string>();
        
        string pattern = @"mm_extra_addons\s+""([^""]*)""";
        Match match = Regex.Match(configContent, pattern);
        
        if (match.Success)
        {
            string idsValue = match.Groups[1].Value.Trim();
            if (!string.IsNullOrEmpty(idsValue))
            {
                string[] ids = idsValue.Split(',');
                workshopIds.AddRange(ids);
            }
        }
        
        return workshopIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .Where(id => IsValidWorkshopId(id))
            .Distinct()
            .ToList();
    }

    private static bool IsValidWorkshopId(string id)
    {
        return !string.IsNullOrWhiteSpace(id) && 
            id.All(char.IsDigit) && 
            !string.IsNullOrEmpty(id);
    }
}

public class VpkParser
{
    public static List<string> GetEventsFromVpk()
    {
        var allEvents = new List<string>();
        List<string> workshopIds = AddonManagerConfig.GetWorkshopIdsFromConfig();

        if (workshopIds.Count == 0)
        {
            return allEvents;
        }

        foreach (string workshopId in workshopIds)
        {
            try
            {
                List<string> Events = GetEventsFromVpk(workshopId);
                allEvents.AddRange(Events);

            }
            catch { }
        }

        return allEvents.Distinct().Select(ConvertVsndevtsExtension).ToList();
    }

    public static List<string> GetEventsFromVpk(string workshopId)
    {
        var Events = new List<string>();

        try
        {
            string vpkPath = FindVpkFile(workshopId);
            if (string.IsNullOrEmpty(vpkPath))
            {
                return Events;
            }


            using var package = new Package();
            package.Read(vpkPath);
            foreach (KeyValuePair<string, List<PackageEntry>> fileType in package.Entries!)
            {

                foreach (PackageEntry entry in fileType.Value)
                {
                    string fullPath = entry.GetFullPath();

                    if (fullPath.EndsWith("_c"))
                        fullPath = fullPath[..^2];

                    if (!Events.Contains(fullPath))
                    {
                        Events.Add(fullPath);
                    }
                }
            }
        }
        catch
        {

        }

        return Events;
    }

    private static string ConvertVsndevtsExtension(string filePath)
    {
        if (filePath.EndsWith("_c"))
        {
            filePath = filePath[..^2];
        }
        return filePath;
    }

    public static string FindVpkFile(string workshopId)
    {
        try
        {
            string startDirectory = MainPlugin.Instance.ModuleDirectory;
            var moduleDir = new DirectoryInfo(startDirectory);
            var gameDir = moduleDir.Parent?.Parent?.Parent?.Parent?.Parent;

            if (gameDir == null || !gameDir.Exists)
                return null!;

            var machine = "linuxsteamrt64";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                machine = "win64";
            }

            string workshopDir = Path.Combine(
                gameDir.FullName,
                "bin",
                machine,
                "steamapps",
                "workshop",
                "content",
                "730",
                workshopId
            );

            if (!Directory.Exists(workshopDir)) return null!;

            string dirVpk = Path.Combine(workshopDir, $"{workshopId}_dir.vpk");
            if (File.Exists(dirVpk)) return dirVpk;

            return Directory.GetFiles(workshopDir, "*.vpk").FirstOrDefault()!;
        }
        catch
        {
            return null!;
        }
    }
}