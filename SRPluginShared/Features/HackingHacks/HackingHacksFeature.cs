using System.Collections.Generic;
using HarmonyLib;

namespace SRPlugin.Features.HackingHacks
{
#if !SRHK
#else
    public class HackingHacksFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIHackingHacksEnabled;
        private static ConfigItem<float> CIHackTimeAdjustment;
        private static ConfigItem<bool> CIAutoHack;

        public HackingHacksFeature()
            : base(
                nameof(HackingHacks),
                [
                    (
                        CIHackingHacksEnabled = new ConfigItem<bool>(
                            PLUGIN_FEATURES_SECTION,
                            nameof(HackingHacks),
                            true,
                            "enables hacking hacks"
                        )
                    ),
                    (
                        CIHackTimeAdjustment = new ConfigItem<float>(
                            nameof(HackTimeAdjustment),
                            2.5f,
                            "hack time multiplier; multiply available starting time by this"
                        )
                    ),
                    (
                        CIAutoHack = new ConfigItem<bool>(
                            nameof(AutoHack),
                            true,
                            "automatically win the hacking minigame; if true, all other settings have no effect"
                        )
                    ),
                ],
                new List<PatchRecord>(
                    PatchRecord.RecordPatches(
                        AccessTools.Method(
                            typeof(MatrixGameClueBarPatch),
                            nameof(MatrixGameClueBarPatch.RestartGamePostfix)
                        ),
                        AccessTools.Method(
                            typeof(MatrixGameClueBarPatch),
                            nameof(MatrixGameClueBarPatch.InitializePostfix)
                        )
                    )
                )
                {
                    }
            ) { }

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

        public static bool AutoHack
        {
            get => CIAutoHack.GetValue();
            set => CIAutoHack.SetValue(value);
        }

        [HarmonyPatch(typeof(MatrixGameClueBar))]
        internal class MatrixGameClueBarPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(MatrixGameClueBar.RestartGame))]
            public static void RestartGamePostfix(MatrixGameClueBar __instance)
            {
                if (HackingHacksEnabled && AutoHack)
                {
                    AccessTools
                        .Method(typeof(MatrixGameClueBar), "CorrectCodeSelected")
                        .Invoke(__instance, new object[] { });
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(MatrixGameClueBar.Initialize))]
            public static void InitializePostfix(ref float ___timeToHack)
            {
                if (HackingHacksEnabled && HackTimeAdjustment > 0f)
                {
                    ___timeToHack *= HackTimeAdjustment;
                }
            }
        }
    }
#endif
}
