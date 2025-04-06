using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text.Json;
using CnD_Sound.Config;
using System.Text.Encodings.Web;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Cvars;

namespace CnD_Sound;

public static class CustomHooks
{
    public static CustomGameData? CustomFunctions { get; set; }
    internal static void StartHook()
    {
        CustomFunctions = new();
        CustomFunctions.CSoundOpGameSystem_SetSoundEventParamFunc_2.Hook( CSoundOpGameSystem_StartSoundEventFunc_2_PostHook, HookMode.Pre );
    }

    internal static void CleanUp()
    {
        CustomFunctions = new();
        CustomFunctions.CSoundOpGameSystem_SetSoundEventParamFunc_2.Unhook( CSoundOpGameSystem_StartSoundEventFunc_2_PostHook, HookMode.Pre );
    }

    public static HookResult CSoundOpGameSystem_StartSoundEventFunc_2_PostHook(DynamicHook hook)
    {
        var hash = hook.GetParam<uint>(3);

        if (hash == 0x2D8464AF)
        {
            hook.SetParam(3, 0xBD6054E9);
        }
        
        return HookResult.Continue;
    }
}