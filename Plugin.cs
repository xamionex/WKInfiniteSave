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
        harmony.PatchAll(typeof(SaveState));
        foreach (var m in harmony.GetPatchedMethods()) Log.LogInfo($"Patched: {m.DeclaringType?.FullName}.{m.Name}");
        Log.LogInfo($"{PluginInfo.PluginName} v{PluginInfo.PluginVersion} loaded - disc saves will no longer be consumed.");
    }
}