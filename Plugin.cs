using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace InfiniteSave
{
    [BepInPlugin(PluginInfo.PluginGuid, PluginInfo.PluginName, PluginInfo.PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        // Master on/off switch.
        internal static ConfigEntry<bool> InfiniteDiscSaves;

        // Prints a small line in the in-game dev console every time a disc save gets protected.
        internal static ConfigEntry<bool> NotifyInConsole;

        private void Awake()
        {
            Log = Logger;

            InfiniteDiscSaves = Config.Bind(
                "General",
                "InfiniteDiscSaves",
                true,
                "When enabled, loading/reviving from a disc save never spends a use. " +
                "The save stays exactly as valid as it was before you loaded it, forever, instead of eventually running out and being deleted.");

            NotifyInConsole = Config.Bind(
                "General",
                "NotifyInConsole",
                true,
                "Print a short message in the in-game developer console every time a disc save is protected from being used up.");

            var harmony = new Harmony(PluginInfo.PluginGuid);
            harmony.PatchAll();

            Log.LogInfo($"{PluginInfo.PluginName} v{PluginInfo.PluginVersion} loaded - disc saves will no longer be consumed.");
        }
    }

    // How disc saves actually work:
    //
    //   App_SavePage.AddSave()      -> creates/tops up a CL_SaveManager.SaveState with type == SaveType.disk and `amount` == the disk's capacity.
    //                                  This is the "insert disk, save" action.
    //
    //   CL_GameManager.Die()        -> on death, looks up the most recent disk-type SaveState and, if one exists, revives you from it instead of ending the run.
    //
    //   App_SavePage.LoadSave()     -> the "load" button in the save terminal does the same lookup manually.
    //
    //   Both paths end up calling CL_SaveManager.SaveState.LoadSave(), an iterator method (a Unity coroutine).
    //   Partway through it, for disk-type saves only, it does:
    //
    //       this.amount--;
    //       if (this.amount <= 0)
    //           CL_SaveManager.saveStates.Remove(this);
    //
    //   That's the entire "use up the disc" mechanic: every load/revive costs one use, and once `amount` hits 0 the save is deleted outright, so the next death is a real game over.
    //   It also directly drives the "X Lives Remaining" text the game shows you right after a revive (via CL_SaveManager.GetNumberOfDiskLives(), which just sums `amount` across every disk-type SaveState).
    //
    //   We can't safely patch that decrement/removal where it happens - it's inside the compiler-generated MoveNext() of that iterator, several `yield`s deep, not inside LoadSave() itself.
    //   But a Prefix on LoadSave() *does* run synchronously right before Unity ever calls MoveNext() on the coroutine it returns - i.e. strictly before any of the method body (including the decrement) has executed.
    //   So bumping `amount` up by one there cancels out the decrement that's about to happen later in the same load, for that exact SaveState instance: the disc's capacity - and the "Lives Remaining" text - never actually goes down, and it never reaches 0, so it's never removed from CL_SaveManager.saveStates.
    //   The disc save behaves as if it's never used.

    [HarmonyPatch(typeof(CL_SaveManager.SaveState), nameof(CL_SaveManager.SaveState.LoadSave))]
    internal static class SaveState_LoadSave_Patch
    {
        private static void Prefix(CL_SaveManager.SaveState __instance)
        {
            if (!Plugin.InfiniteDiscSaves.Value)
            {
                return;
            }

            if (__instance == null || __instance.type != CL_SaveManager.SaveState.SaveType.disk)
            {
                return;
            }

            __instance.amount++;

            if (Plugin.NotifyInConsole.Value)
            {
                CommandConsole.Log("<color=grey>[InfiniteSave] Disc save use restored - this save will not run out.</color>");
            }

            Plugin.Log.LogInfo($"Protected disc save '{__instance.id}' from being consumed (amount bumped to {__instance.amount} before load).");
        }
    }
}
