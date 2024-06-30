# Thoughts on the current design

## Features

I'm not sold on the design as it stands. What I wanted, if you couldn't tell, were patch categories, but that didn't work out. I would also have loved the ability to call Harmony.UnpatchAll(Type) and it will unpatch all patches that refer to a HarmonyPatch from that class.

That said, it's got flaws. First, it seems like a solution looking for a problem. A concern I sort of feel for this project as a whole. Anyway, it's only critical if I decide I want to support bundles of features to be disabled during play, as opposed to having a config file updated and the game restarted.
I think the more common case would be someone setting which features they want and probably not touching them again. Still, it's not a required part of the code to operate; if you switch to call Harmony.PatchAll() at startup, you'll just pick up all Harmony patches available. If that's satisfactory, then you can ignore
all of this goofy Feature nonsense.

## Cheatier
Then I remembered I had hacked around with dnSpy and added a couple of extra buttons to my DebugConsole.DrawCheatBar, that granted tons more karma and tons more nuyen.

Then I thought I should add to the Cheat Bar. Then I figured it might be easier to add "cheatier" bar below the Cheat bar (which is always last for my game) with just my cheats.

At which point I realized I could *probably* make all of these things toggleable, but at the moment an attempt is made not to arbitrarily hook things that don't need to be hooked. Er... patched.

So having cheat buttons that grant the current player some bigger points is no big deal. It's a little weirder arbitrarily drawing myself below the normal cheat bar.

##
I keep playing with the "Features" thing. I'm goofing around with refactoring as I want to get to the point where you just create a new Feature implementation and you're good to go.