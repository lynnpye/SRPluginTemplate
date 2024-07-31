using HarmonyLib;
using isogame;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRPlugin.Features.HackingHacks
{
#if !SRHK
#else
    public class HackingHacksFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIHackingHacksEnabled;
        private static ConfigItem<float> CIHackTimeAdjustment;
        private static ConfigItem<float> CIPenaltyTimeAdjustment;
        private static ConfigItem<bool> CIAutoHack;

        public HackingHacksFeature()
            : base(
                  nameof(HackingHacks),
                  [
                      (CIHackingHacksEnabled = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(HackingHacks), true, "enables hacking hacks" )),
                      (CIHackTimeAdjustment = new ConfigItem<float>(nameof(HackTimeAdjustment), 2.5f, "hack time multiplier; multiply available starting time by this")),
                      (CIPenaltyTimeAdjustment = new ConfigItem<float>(nameof(PenaltyTimeAdjustment), 0.25f, "penalty time multiplier; multiply penalty time deducted by this")),
                      (CIAutoHack = new ConfigItem<bool>(nameof(AutoHack), false, "automatically win the hacking minigame; if true, all other settings have no effect")),
                  ],
                  new List<PatchRecord>(
                      PatchRecord.RecordPatches(
                        )
                  )
                  { }
                  )
        { }

        public static bool HackingHacksEnabled
        {
            get => CIHackingHacksEnabled.GetValue();
            set => CIHackingHacksEnabled.SetValue(value);
        }

        public static float HackTimeAdjustment
        {
            get => CIHackTimeAdjustment.GetValue();
            set => CIHackTimeAdjustment.SetValue(value);
        }

        public static float PenaltyTimeAdjustment
        {
            get => CIPenaltyTimeAdjustment.GetValue();
            set => CIPenaltyTimeAdjustment.SetValue(value);
        }

        public static bool AutoHack
        {
            get => CIAutoHack.GetValue();
            set => CIAutoHack.SetValue(value);
        }

        [HarmonyPatch(typeof(BlockerICPanelParameters))]
        internal class BlockerICPanelParametersPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(BlockerICPanelParameters.time_to_hack), MethodType.Getter)]
            public static void time_to_hackPostfix(ref float __result)
            {
                if (HackingHacksEnabled)
                {
                    __result *= HackTimeAdjustment;
                }
            }
        }

        [HarmonyPatch(typeof(MatrixGameClueBar))]
        internal class MatrixGameClueBarPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("DoTimePenalty")]
            public static void DoTimePenaltyPrefix(ref float timeToRemove)
            {
                Squawk($"How many times will this get called for one failure?");
                if (HackingHacksEnabled)
                {
                    timeToRemove *= PenaltyTimeAdjustment;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(MatrixGameClueBar.RestartGame))]
            public static void RestartGamePostfix(MatrixGameClueBar __instance)
            {
                if (HackingHacksEnabled && AutoHack)
                {
                    AccessTools.Method(typeof(MatrixGameClueBar), "CorrectCodeSelected").Invoke(__instance, new object[] { });
                }
            }
        }
    }
#endif
}
