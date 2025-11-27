using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CnD_Sound;

public static class CustomHooks
{
    public static CustomGameData? CustomFunctions { get; private set; }
    private static bool _isHooked = false;

    internal static void StartHook()
    {
        if (_isHooked) return;
        
        CustomFunctions = new CustomGameData();
        _isHooked = true;
        Helper.DebugMessage("Hooks Started Successfully");
    }

    internal static void UnHook()
    {
        if (!_isHooked || CustomFunctions == null) return;
        
        if (CustomFunctions.CSoundOpGameSystem_SetSoundEventParamFunc_2 != null)
        {
            CustomFunctions.CSoundOpGameSystem_SetSoundEventParamFunc_2.Unhook(CSoundOpGameSystem_StartSoundEventFunc_2_PreHook, HookMode.Pre);
        }

        CustomFunctions = null;
        _isHooked = false;
        Helper.DebugMessage("Hooks Removed Successfully");
    }

    public static HookResult CSoundOpGameSystem_StartSoundEventFunc_2_PreHook(DynamicHook hook)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var hash = hook.GetParam<uint>(3);
            if (hash == 0x2D8464AF)
            {
                hook.SetParam(3, 0xBD6054E9);
            }
        }
        else
        {
            var hash = hook.GetParam<uint>(2);
            if (hash == 0x2D8464AF)
            {
                hook.SetParam(2, 0xBD6054E9);
            }
        }

        return HookResult.Continue;
    }
}