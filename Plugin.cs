using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace InfiniteSave;

[BepInPlugin(PluginInfo.PluginGuid, PluginInfo.PluginName, PluginInfo.PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource Log;
    public static ConfigEntry<bool> InfiniteDiscSaves;
    private void Awake()
    {
        Log = Logger;

        InfiniteDiscSaves = Config.Bind(
            "General",
            "InfiniteDiscSaves",
            true,
            "When enabled, loading/reviving from a disc save never spends a use.");

        var harmony = new Harmony(PluginInfo.PluginGuid);
        harmony.PatchAll();

        Log.LogInfo($"{PluginInfo.PluginName} v{PluginInfo.PluginVersion} loaded - disc saves will no longer be consumed.");
    }
    
    [HarmonyPatch(typeof(CL_SaveManager.SaveState), nameof(CL_SaveManager.SaveState.LoadSave))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static void SaveStateLoadSavePatch(CL_SaveManager.SaveState __instance)
    {
        if (!InfiniteDiscSaves.Value || __instance is not { type: CL_SaveManager.SaveState.SaveType.disk }) return;
        __instance.amount++;
        CommandConsole.Log("<color=grey>[InfiniteSave] Disc save use restored - this save will not run out.</color>");
        Log.LogInfo($"Protected disc save '{__instance.id}' from being consumed (amount bumped to {__instance.amount} before load).");
    }
}