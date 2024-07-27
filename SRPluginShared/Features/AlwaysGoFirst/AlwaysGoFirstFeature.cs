using System.Collections.Generic;
using HarmonyLib;

namespace SRPlugin.Features.AlwaysGoFirst
{
    public class AlwaysGoFirstFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIAlwaysGoFirst;

        public AlwaysGoFirstFeature()
            : base(
                nameof(AlwaysGoFirst),
                [
                    (
                        CIAlwaysGoFirst = new ConfigItem<bool>(
                            PLUGIN_FEATURES_SECTION,
                            nameof(AlwaysGoFirst),
                            true,
                            "your team will always go first in combat"
                        )
                    ),
                ],
                new List<PatchRecord>(
                    PatchRecord.RecordPatches(
                        AccessTools.Method(
                            typeof(TurnDirectorPatch),
                            nameof(TurnDirectorPatch.TeamRatingPostfix)
                        )
                    )
                )
                {
                    }
            ) { }

        public static bool AlwaysGoFirst
        {
            get => CIAlwaysGoFirst.GetValue();
            set => CIAlwaysGoFirst.SetValue(value);
        }

        public override void PostApplyPatches()
        {
            combatEventListener = new CombatEventListener();

            KeyMessenger<TileGrid, Grid_FriendlyWorld_Event>.AddGlobalListener(
                new KeyMessenger<TileGrid, Grid_FriendlyWorld_Event>.KeyDelegate(
                    combatEventListener.OnFriendlyWorldChanged
                )
            );
            KeyMessenger<TileGrid, Grid_CombatWorld_Event>.AddGlobalListener(
                new KeyMessenger<TileGrid, Grid_CombatWorld_Event>.KeyDelegate(
                    combatEventListener.OnCombatWorldChanged
                )
            );
        }

        public override void HandleDisabled()
        {
            if (combatEventListener == null)
            {
                return;
            }
            combatEventListener = null;
        }

        private static bool RigRating { get; set; }
        private static CombatEventListener combatEventListener = null;

        private class CombatEventListener
        {
            private bool inCombat = false;

            private void SetCombatState(bool isFriendly)
            {
                if (!inCombat && !isFriendly)
                {
                    RigRating = true;
                    inCombat = true;
                }
                else if (isFriendly)
                {
                    RigRating = false;
                    inCombat = false;
                }
            }

            public void OnFriendlyWorldChanged(TileGrid grid, Grid_FriendlyWorld_Event e)
            {
                SetCombatState(e.isFriendlyWorld);
            }

            public void OnCombatWorldChanged(TileGrid grid, Grid_CombatWorld_Event e)
            {
                SetCombatState(!e.isCombatWorld);
            }
        }

        [HarmonyPatch(typeof(TurnDirector))]
        internal class TurnDirectorPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(TurnDirector.TeamRating))]
            public static void TeamRatingPostfix(ref int __result, Team t)
            {
                if (!AlwaysGoFirst || !RigRating)
                {
                    return;
                }

                if (AccessTools.Field(typeof(Team), "units").GetValue(t) is not List<Player> units)
                {
                    return;
                }

                foreach (Player u in units)
                {
                    if (
                        u.actorUID
                        == SceneSingletonBehavior<TurnDirector>.Instance.PlayerZero.actorUID
                    )
                    {
                        SRPlugin.Squawk($"rigging team rating");
                        RigRating = false;
                        __result = int.MaxValue;
                        return;
                    }
                }
            }
        }
    }
}
