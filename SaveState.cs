using HarmonyLib;

namespace InfiniteSave;

internal static class SaveState
{
    [HarmonyPatch(typeof(CL_SaveManager.SaveState), nameof(CL_SaveManager.SaveState.LoadSave))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static void SaveStateLoadSavePatch(CL_SaveManager.SaveState __instance)
    {
        if (!Plugin.InfiniteDiscSaves.Value || __instance is not { type: CL_SaveManager.SaveState.SaveType.disk }) return;
        __instance.amount++;
        CommandConsole.Log("<color=grey>[InfiniteSave] Disc save use restored - this save will not run out.</color>");
        Plugin.Log.LogInfo($"Protected disc save '{__instance.id}' from being consumed (amount bumped to {__instance.amount} before load).");
    }
}