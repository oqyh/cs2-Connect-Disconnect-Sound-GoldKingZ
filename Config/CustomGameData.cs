using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CnD_Sound.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CnD_Sound;

public class CustomGameData
{

    public static Dictionary<string, Dictionary<OSPlatform, string>> _customGameData = new();
    public readonly MemoryFunctionWithReturn<nint, nint, nint, uint, nint, uint, uint, byte> CSoundOpGameSystem_SetSoundEventParamFunc_2;

    public CustomGameData()
    {
        LoadCustomGameDataFromJson();

        CSoundOpGameSystem_SetSoundEventParamFunc_2 = new(GetCustomGameDataKey("CSoundOpGameSystem_SetSoundEventParamFunc_2"));
    }
    public void LoadCustomGameDataFromJson()
    {
        string jsonFilePath = Path.Combine(MainPlugin.Instance.ModuleDirectory, "gamedata/gamedata.json");
        if (!File.Exists(jsonFilePath))
        {
            Helper.DebugMessage($"JSON file does not exist at path: {jsonFilePath}. Returning without loading custom game data.");
            return;
        }
        
        try
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            var jsonObject = JObject.Parse(jsonData);

            foreach (var item in jsonObject.Properties())
            {
                string key = item.Name;
                var platformData = new Dictionary<OSPlatform, string>();
                var signatures = item.Value["signatures"];
                if (signatures != null)
                {
                    if (signatures["windows"] != null)
                    {
                        platformData[OSPlatform.Windows] = signatures["windows"]!.ToString();
                    }
                    if (signatures["linux"] != null)
                    {
                        platformData[OSPlatform.Linux] = signatures["linux"]!.ToString();
                    }
                }

                _customGameData[key] = platformData;
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Error loading custom game data: {ex.Message}");
        }
    }


    public string GetCustomGameDataKey(string key)
    {
        if (!_customGameData.TryGetValue(key, out var customGameData))
        {
            Helper.DebugMessage($"Invalid key {key}. Throwing exception.");
            throw new Exception($"Invalid key {key}");
        }

        OSPlatform platform;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            platform = OSPlatform.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            platform = OSPlatform.Windows;
        }
        else
        {
            Helper.DebugMessage("Unsupported platform. Throwing exception.");
            throw new Exception("Unsupported platform");
        }

        return customGameData.TryGetValue(platform, out var customData)
            ? customData
            : throw new Exception($"Missing custom data for {key} on {platform}");
    }
}