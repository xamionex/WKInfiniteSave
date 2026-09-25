# InfiniteSave

A BepInEx mod for **White Knuckle**.
Makes disc saves last forever: loading from a disc save (manually, or via the auto-revive on death) never spends a use, so the save never runs out and never gets deleted. \
**You still have to save to a disc the normal way first, this only stops that save from being consumed afterward.**

Made with the same template/workflow as my other mods (https://github.com/xamionex/SaltAndSacrificeBepInExTemplate), adapted for White Knuckle.

## Building

1. Install the [.NET SDK](https://dotnet.microsoft.com/download) (net462 target).
2. Drop the following DLLs into `lib/`:
   - `Assembly-CSharp.dll`
   - `BepInEx.dll`
   - `0Harmony.dll`
3. (Optional) Create a `.gamelocation` file in the dir containing a single line, the path to your White Knuckle install (or your Gale/r2modman profile folder) and the build will auto-copy the built plugin into `BepInEx/plugins/InfiniteSave/` for you.
4. `dotnet build` (or open `InfiniteSave.sln` in Rider/Visual Studio and build there).

## Installation

- Install via [gale](https://old.thunderstore.io/c/white-knuckle/p/Kesomannen/GaleModManager/)
