# SRPluginTemplate
BepInEx based mod template

## About this Plugin
Despite being called a 'template' it includes a lot of actual functionality, which I use to expand on the "Features" stuff I'm playing with. Currently, this plugin offers the following features:

|Feature|Description|SRR|DFDC|SRHK|
|---|---|---|---|---|
| AlwaysBeSprinting | Your character and anyone following you in party mode will move at a brisk sprint in all cases. This feature sets the global threshold for when to switch speeds up on walk and run to 0, effectively making your character sprint everywhere. QoL improvement.|X|X|X|
|Cheatier|Adds an extra 'Cheatier' cheats menu bar when using the built-in Alt-F1 > Cheats option. The additional Cheatier features are described below.|X|X|X|
|CyberwareAffinityEssenceBonusOverride|The default essence bonuses from the base game (i.e. +1 essence at CA3 and +1 more (+2 total) at CA6) would be "0 0 1 1 1 2". This lets you change that.|N/A|N/A|X|
|FixCharacterSheetArmor|Fixes a display bug in the base game. The PDA obtains a cached copy of Player data which isn't always updated appropriately, for instance after getting cyberware installed or purchasing and applying a new outfit from a merchant. This resulted in incorrectly, typically low, armor values showing on the PDA character screen. This was caused after SRR when they added armor "damage" in addition to armor "penetration". This fix tries to properly update the stats and also adds a current/max armor display change on the character sheet.|N/A|X|X|
|MaxAttributesOverride|Originally MaxAttributes20, this feature allows you to set the racial maximums for each attribute and race combination, adjusting the overall GetAttributeMax as a result. It does default everything to 20, but if you don't want to play god-tier, but see no reason why Trolls need a lower max Intelligence than everyone else, you can live your dream.|X|X|X|
|NoCostCyberware|Adding cyberware incurs no essence costs.|X|X|X|
|NoUnspentKarmaPopup|Prevents the popup at the start of a scene telling you you have unspent karma and asking if you might want to spend it now. The kind of thing it would be nice to show on an options UI.|X|X|X|
|OverrideStartingKarma|The karma you start a new character with. With max attributes all set to 20, you need almost 5500. I default to 60. The game default is 5. It allows 0 (meaning no starting karma), but values below 0, while allowed, disable the feature. It will remain patched but will not actually make any changes.|X|X|X|
|ReduceSpiritEscape|Lets you override the values that add to the escape prevention roll for both distance from your spirit as well as how many AP it has been granted. Can make it easier or harder.|X|X|X|

The only feature-to-feature dependency is for NoCostCyberware to properly calculate the essence bonus from Cyberware Affinity, it requests that directly from the CyberwareAffinityEssenceBonusOverride feature because
the base game doesn't have a way to retrieve it from the API; it's just sort of hard-coded in/implemented in a few spots, like the karma screen, the calculation of "derived" essence (i.e. impacted by things like cyberware,
but not also by Cyberware Affinity) and implicitly in the GetSkillMax calls, but that's more of a patch to fix an existing bug than anything else.

### Cheatier Functions
Cheatier adds an additional cheat bar with the following functions:

|Cheatier Function|Description|
|---|---|
|Stash!|Grants access to your equipment stash. Might be a cool item to add, like a one time use equipment drone.|
|+1APMax|Gives a permanent +1 increase to max Action Points.|
|+100AP|Gives 100 Action Points to spend in the current turn. Once your turn ends, you'll be back to your normal Action Point pool.|
|+50HPMax|Gives a permanent +50 increase to max HP.|
|+500HP|Adds 500 HP to current total, with overheal (i.e. bonus health that will be damaged first)|
|+1KK|Gives 1000 karma. With all stat maxes at 20, it takes just under 5,500 karma to cap everything.|
|+100k¥|Gives 100,000 nuyen.|


# To use this template

## Install BepInEx
Go to the [BepInEx Releases page](https://github.com/BepInEx/BepInEx/releases/) and download BepInEx_win_x86_5.4.23.2.zip [(link to the release tag)](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2) [(direct link to the zip)](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x86_5.4.23.2.zip) and install it to your game folder as normal.

Run the game once now that BepInEx is installed. The game will likely crash. If it does not, exit the game as soon as the main menu appears.

When installing BepInEx to the game folder, a 'BepInEx/' folder was created. There should now be a 'config/' subfolder containing 'BepInEx.cfg'.

Edit 'BepInEx.cfg', finding the [Preloader.Entrypoint] section and changing the 'Type' value to 'Camera'. That section should now look like this:

```
[Preloader.Entrypoint]

## The local filename of the assembly to target.
# Setting type: String
# Default value: UnityEngine.dll
Assembly = UnityEngine.dll

## The name of the type in the entrypoint assembly to search for the entrypoint method.
# Setting type: String
# Default value: Application
Type = Camera

## The name of the method in the specified entrypoint assembly and type to hook and load Chainloader from.
# Setting type: String
# Default value: .cctor
Method = .cctor
```

## Developing your BepInEx plugin (i.e. your mod)
### For Shadowrun Returns
Add a new Environment Variable (personal or system, doesn't matter):
	SRRInstallDir
	<The folder where your Shadowrun.exe is located>
So if your Shadowrun.exe is located at "C:\Program Files (x86)\Steam\steamapps\common\Shadowrun Returns\Shadowrun.exe", then SRRInstallDir would be set to "C:\Program Files (x86)\Steam\steamapps\common\Shadowrun Returns".

### For Shadowrun Dragonfall Director's Cut
Add a new Environment Variable (personal or system, doesn't matter):
	DFDCInstallDir
	<The folder where your Dragonfall.exe is located>
So if your Dragonfall.exe is located at "C:\Program Files (x86)\Steam\steamapps\common\Shadowrun Dragonfall Director's Cut\Dragonfall.exe", then DFDCInstallDir would be set to "C:\Program Files (x86)\Steam\steamapps\common\Shadowrun Dragonfall Director's Cut".

### For Shadowrun Hong Kong
Add a new Environment Variable (personal or system, doesn't matter):
	SRHKInstallDir
	<The folder where your SRHK.exe is located>
So if your SRHK.exe is located at "C:\Program Files (x86)\Steam\steamapps\common\Shadowrun Hong Kong\SRHK.exe", then SRHKInstallDir would be set to "C:\Program Files (x86)\Steam\steamapps\common\Shadowrun Hong Kong".

### Selective compilation
While the SRPluginShared code is mostly compatible across versions, there are of course some difference, so when you are creating patches, you may need to opt to exclude them from one or two of the three game versions. I'm using preprocessor directives with functional names.

* NARROWKARMABUTTONS - This directive attempts to scale the karma screen buttons to be more narrow to fit all 20 in horizontally. Only of interest when MaxAttributes20 feature is enabled.

### And then
Download this template and edit things like project names, versions, etc. You may need to edit the .csproj manually.

Create your plugin per guidelines from HarmonyX and BepInEx.

Install the built .dll to the BepInEx/plugins folder to have it take effect.

Congratulations, you are modding Shadowrun without using dnSpy to edit the Assembly-CSharp.dll directly.

## More notes
Steps I went through to arrive at this template...
... actually I did most of this with SRHKPlugin, and copied it over and applied it to Dragonfall, but it all worked out remarkably similarly. But sure, continue reading :) ...

As mentioned above, I started with the BepInEx_win_x86_5.4.23.2.zip file and installed it.

I created the environment variable because I use it in the .csproj to create relative references to ShadowrunDTO.dll (which contains a lot of types) and Assembly-CSharp.dll (which contains all the logic).

I initially was following steps for creating BepInEx plugins from their [setup](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/1_setup.html) and [plugin start](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/2_plugin_start.html) pages.

I ran into a problem when trying to run 'dotnet new bepinex5plugin...' with the error:
```
Template "BepInEx 5 Plugin Template" could not be created.
Failed to create template.
Details: Object reference not set to an instance of an object.
```

Whereupon I followed the steps on [this StackOverflow page](https://stackoverflow.com/questions/42077229/switch-between-dotnet-core-sdk-versions/42078060#42078060) to create a global.json. It turns out I had .NET 8 on my machine, had to install .NET 7 (7.0.410), and then was able to run the command above to generate the template.

I was then able to delete the global.json file.

Initially all that is created is the .csproj and source files, no .sln file. When you open the .csproj and then click File>Close Solution, it asks you if you wish to save the .sln. I did. When I had first opened the .csproj, the BepInEx NuGet source URL was already set up. When I reopened the .sln file, it was missing and I had to manually add it via Tools>NuGet Package Manager>Manage NuGet Packages for Solution.

That URL is https://nuget.bepinex.dev/v3/index.json

I also manually modified the .csproj file to add references to the two DLLs that ship with the game. As I mentioned above, I use the environment variable to make the referencing easier. I also had to mark Assembly-CSharp.dll as ExternallyResolved=True because there is a version mismatch on mscorlib.dll between the target framework in the project and what is shipped with the game.

## And then
I've since created a single solution setup with projects for both Shadowrun Hong Kong and Shadowrun Dragonfall Director's Cut. So far it seems to work well enough.