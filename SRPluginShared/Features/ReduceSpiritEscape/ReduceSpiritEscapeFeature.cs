﻿using HarmonyLib;
using System.Collections.Generic;

namespace SRPlugin.Features.ReduceSpiritEscape
{
    public class ReduceSpiritEscapeFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIReduceSpiritEscape;
        private static ConfigItem<float> CISummonDistancePenalty;
        private static ConfigItem<float> CISummonAPRefreshPenalty;

        public ReduceSpiritEscapeFeature()
            : base(new List<ConfigItemBase>()
            {
                (CIReduceSpiritEscape = new ConfigItem<bool>(FEATURES_SECTION, nameof(ReduceSpiritEscape), true, "decreases the likelihood of a spirit escaping control")),
                (CISummonDistancePenalty = new ConfigItem<float>(FEATURES_SECTION, nameof(SummonDistancePenalty), 0.5f, "game default is 1.0; lower values reduce summon escape penalty due to distance")),
                (CISummonAPRefreshPenalty = new ConfigItem<float>(FEATURES_SECTION, nameof(SummonAPRefreshPenalty), 0.5f, "game default is 1.0; lower values reduce summon escape penalty due to AP refreshes"))
            }, new List<PatchRecord>()
            {
                PatchRecord.Postfix(
                    typeof(Constants).GetMethod(nameof(Constants.LoadDefaults)),
                    typeof(ConstantsPatch).GetMethod(nameof(ConstantsPatch.LoadDefaultsPostfix))
                    )
            })
        {

        }

        public override void HandleDisabled()
        {
            ResetStartingValues();
        }

        public static bool ReduceSpiritEscape { get => CIReduceSpiritEscape.GetValue(); set => CIReduceSpiritEscape.SetValue(value); }
        public static float SummonDistancePenalty { get => CISummonDistancePenalty.GetValue(); set => CISummonDistancePenalty.SetValue(value); }
        public static float SummonAPRefreshPenalty { get => CISummonAPRefreshPenalty.GetValue(); set => CISummonAPRefreshPenalty.SetValue(value); }

        private static OverrideableValue<float> OVSummonDistancePenalty = new OverrideableValue<float>(Constants.SHAMAN_DISTANCE_PENALTY_MOD, (v) => Constants.SHAMAN_DISTANCE_PENALTY_MOD = v);
        private static OverrideableValue<float> OVSummonAPRefreshPenalty = new OverrideableValue<float>(Constants.SHAMAN_AP_REFRESH_PENALTY_MOD, (v) => Constants.SHAMAN_AP_REFRESH_PENALTY_MOD = v);

        public static void ResetStartingValues()
        {
            OVSummonDistancePenalty.Reset();
            OVSummonAPRefreshPenalty.Reset();
        }

        public static void ApplyOverrideValues()
        {
            if (!ReduceSpiritEscape) return;

            if (SummonDistancePenalty >= 0)
            {
                OVSummonDistancePenalty.Set(SummonDistancePenalty);
            }

            if (SummonAPRefreshPenalty >= 0)
            {
                OVSummonAPRefreshPenalty.Set(SummonAPRefreshPenalty);
            }
        }

        [HarmonyPatch(typeof(Constants))]
        internal class ConstantsPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(Constants.LoadDefaults))]
            public static void LoadDefaultsPostfix()
            {
                ApplyOverrideValues();
            }
        }
    }
}
