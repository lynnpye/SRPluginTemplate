using HarmonyLib;
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
            })
        {

        }

        public override void HandleEnabled()
        {
            SRPlugin.Harmony.PatchAll(typeof(ConstantsPatch));
        }

        public override void HandleDisabled()
        {
            SRPlugin.Harmony.Unpatch(
                typeof(Constants).GetMethod(nameof(Constants.LoadDefaults)),
                typeof(ConstantsPatch).GetMethod(nameof(ConstantsPatch.LoadDefaultsPatch))
                );

            ResetStartingValues();
        }

        public static bool ReduceSpiritEscape
        {
            get
            {
                return CIReduceSpiritEscape.GetValue();
            }

            set
            {
                CIReduceSpiritEscape.SetValue(value);
            }
        }

        public static float SummonDistancePenalty
        {
            get
            {
                return CISummonDistancePenalty.GetValue();
            }

            set
            {
                CISummonDistancePenalty.SetValue(value);
            }
        }

        public static float SummonAPRefreshPenalty
        {
            get
            {
                return CISummonAPRefreshPenalty.GetValue();
            }

            set
            {
                CISummonAPRefreshPenalty.SetValue(value);
            }
        }

        private static float? ORIGINAL_SUMMON_DISTANCE_PENALTY = null;
        private static float? ORIGINAL_SUMMON_AP_REFRESH_PENALTY = null;

        public static void ResetStartingValues()
        {
            if (ORIGINAL_SUMMON_DISTANCE_PENALTY.HasValue)
            {
                Constants.SHAMAN_DISTANCE_PENALTY_MOD = ORIGINAL_SUMMON_DISTANCE_PENALTY.Value;
                ORIGINAL_SUMMON_DISTANCE_PENALTY = null;
            }

            if (ORIGINAL_SUMMON_AP_REFRESH_PENALTY.HasValue)
            {
                Constants.SHAMAN_AP_REFRESH_PENALTY_MOD = ORIGINAL_SUMMON_AP_REFRESH_PENALTY.Value;
                ORIGINAL_SUMMON_AP_REFRESH_PENALTY = null;
            }
        }

        public static void ApplyOverrideValues()
        {
            if (!ReduceSpiritEscape) return;

            if (SummonDistancePenalty >= 0)
            {
                if (!ORIGINAL_SUMMON_DISTANCE_PENALTY.HasValue)
                {
                    ORIGINAL_SUMMON_DISTANCE_PENALTY = Constants.SHAMAN_DISTANCE_PENALTY_MOD;
                }
                Constants.SHAMAN_DISTANCE_PENALTY_MOD = SummonDistancePenalty;
            }

            if (SummonAPRefreshPenalty >= 0)
            {
                if (!ORIGINAL_SUMMON_AP_REFRESH_PENALTY.HasValue)
                {
                    ORIGINAL_SUMMON_AP_REFRESH_PENALTY = Constants.SHAMAN_AP_REFRESH_PENALTY_MOD;
                }
                Constants.SHAMAN_AP_REFRESH_PENALTY_MOD = SummonAPRefreshPenalty;
            }
        }

        [HarmonyPatch(typeof(Constants))]
        internal class ConstantsPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(Constants.LoadDefaults))]
            public static void LoadDefaultsPatch()
            {
                ApplyOverrideValues();
            }
        }
    }
}
