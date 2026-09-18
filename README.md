# InfiniteSave

A BepInEx mod for **White Knuckle**.
Makes disc saves last forever: loading from a disc save (manually, or via the auto-revive on death) never spends a use, so the save never runs out and never gets deleted.
You still have to save to a disc the normal way first, this only stops that save from being *consumed* afterwards.

Made with the same template/workflow as my other mods (https://github.com/xamionex/SaltAndSacrificeBepInExTemplate), adapted for White Knuckle.

## Building

1. Install the [.NET SDK](https://dotnet.microsoft.com/download) (net462 target).
2. Drop the following DLLs into `lib/` (see `lib/README.txt`):
   - `Assembly-CSharp.dll`
   - `BepInEx.dll`
   - `0Harmony.dll`
3. (Optional) Create a `.gamelocation` file in the dir containing a single line, the path to your White Knuckle install (or your Gale/r2modman profile folder) and the build will auto-copy the built plugin into `BepInEx/plugins/InfiniteSave/` for you.
4. `dotnet build` (or open `InfiniteSave.sln` in Rider/Visual Studio and build there).

## Installation (pre-built)

- Install [BepInEx 5](https://github.com/BepInEx/BepInEx) for White Knuckle.
- Drop `amione.InfiniteSave.dll` into `White Knuckle/BepInEx/plugins/InfiniteSave/`.

## Config

`BepInEx/config/amione.InfiniteSave.cfg` (auto-created on first run), or the in-game Mods menu if [WKModMenu](https://github.com/monksilly/WKModMenu) is installed:

- **InfiniteDiscSaves** (default `true`) - master on/off switch.
- **NotifyInConsole** (default `true`) - prints a small confirmation line in the in-game dev console every time a disc save is protected.
