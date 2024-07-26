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
            KeyMessenger<string, global::GenericArgs>.AddKeyListener(
                GenericEvents.EVENT_ONCOMBATENTERED,
                new KeyMessenger<string, global::GenericArgs>.KeyDelegate(
                    combatEventListener.OnCombatEntered
                )
            );
            KeyMessenger<string, global::GenericArgs>.AddKeyListener(
                GenericEvents.EVENT_ONCOMBATEXITED,
                new KeyMessenger<string, global::GenericArgs>.KeyDelegate(
                    combatEventListener.OnCombatExited
                )
            );
#if SRHK
            KeyMessenger<string, global::GenericArgs>.AddKeyListener(
                GenericEvents.EVENT_ONCOMBATINTENSE,
                new KeyMessenger<string, global::GenericArgs>.KeyDelegate(
                    combatEventListener.OnCombatIntense
                )
            );
            KeyMessenger<string, global::GenericArgs>.AddKeyListener(
                GenericEvents.EVENT_ONCOMBATWRAPUP,
                new KeyMessenger<string, global::GenericArgs>.KeyDelegate(
                    combatEventListener.OnCombatWrapup
                )
            );
#endif
        }

        public override void HandleDisabled()
        {
            if (combatEventListener == null)
            {
                return;
            }
#if SRHK
            KeyMessenger<string, global::GenericArgs>.RemoveKeyListener(
                GenericEvents.EVENT_ONCOMBATWRAPUP,
                new KeyMessenger<string, global::GenericArgs>.KeyDelegate(
                    combatEventListener.OnCombatWrapup
                )
            );
            KeyMessenger<string, global::GenericArgs>.RemoveKeyListener(
                GenericEvents.EVENT_ONCOMBATINTENSE,
                new KeyMessenger<string, global::GenericArgs>.KeyDelegate(
                    combatEventListener.OnCombatIntense
                )
            );
#endif
            KeyMessenger<string, global::GenericArgs>.RemoveKeyListener(
                GenericEvents.EVENT_ONCOMBATEXITED,
                new KeyMessenger<string, global::GenericArgs>.KeyDelegate(
                    combatEventListener.OnCombatExited
                )
            );
            KeyMessenger<string, global::GenericArgs>.RemoveKeyListener(
                GenericEvents.EVENT_ONCOMBATENTERED,
                new KeyMessenger<string, global::GenericArgs>.KeyDelegate(
                    combatEventListener.OnCombatEntered
                )
            );
            combatEventListener = null;
        }

        private static bool RigRating { get; set; }
        private static CombatEventListener combatEventListener = null;

        private class CombatEventListener
        {
            public void OnCombatEntered(string key, global::GenericArgs inputs)
            {
                RigRating = true;
            }

            public void OnCombatExited(string key, global::GenericArgs inputs)
            {
                RigRating = false;
            }

#if SRHK
            public void OnCombatIntense(string key, global::GenericArgs inputs)
            {
                RigRating = true;
            }

            public void OnCombatWrapup(string key, global::GenericArgs inputs)
            {
                RigRating = false;
            }
#endif
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
