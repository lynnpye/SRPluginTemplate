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

##
Oh yeah, need to get rid of those pesky "Unspent Karma" popups at the beginning of new levels. I take full responsibility for how my karma is spent at this point.

And what about a way to dynamically add assets? Without having to rebake them into a new campaign.

But what if it was in an optional plugin that didn't alter the original DLL, thus allowing other possibly otherwise conflicting additional mods to be installed and played with too, and thus enhancing my overall gameplay experience.
*cough*
You could just.... create a new score and release it, for someone to use on a play through the Dragonfall campaign, knowing they could do so without it otherwise affecting anything else they're doing. Or not, but that's on them. :)

Eh... it's probably already done. Seriously, I guess I'm just a little surprised that the BepInEx thing never took root so far as I can tell. I see inquiries in a handful of places but I've not seen any indication it was routinely used.

In the community for a game where hacking is so prevalent.

I'm also a developer. I dunno, I guess I thought it was funny.

Oh, and yeah, so apparently in SRHK in the cyberware screen, on subsequent passes through, you will see yourself capped at 6 Essence instead of 7 or more with things like Cyber Affinity. I mean... as far as I can tell, the appropriate block of code is returning the appropriate value.
So I'm still working out what's going on in SRHK.

Formatting a new external drive. Did not choose Quick since it's the first time. Hang onto your hats ladies and gentlemen, we are at 4%.

And holding.

