using HarmonyLib;
using System.Collections.Generic;

namespace SRPlugin.Features.OverrideStartingKarma
{
    internal class OverrideStartingKarmaFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIEnableOverrideStartingKarma;
        private static ConfigItem<int> CIOverrideStartingKarma;

        public OverrideStartingKarmaFeature()
            : base(new List<ConfigItemBase>()
            {
                (CIEnableOverrideStartingKarma = new ConfigItem<bool>(FEATURES_SECTION, nameof(EnableOverrideStartingKarma), true, "setting to false prevents patching")),
                (CIOverrideStartingKarma = new ConfigItem<int>(FEATURES_SECTION, nameof(OverrideStartingKarma), 60, "game default is 5; -1 also disables even with the feature enabled, but still patched"))
            }, new List<PatchRecord>()
            {
                PatchRecord.Prefix(
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

        public static int OverrideStartingKarma { get => CIOverrideStartingKarma.GetValue(); set => CIOverrideStartingKarma.SetValue(value); }
        public static bool EnableOverrideStartingKarma { get => CIEnableOverrideStartingKarma.GetValue(); set => CIEnableOverrideStartingKarma.SetValue(value); }

        private static OverrideableValue<int> OVStartingKarma = new OverrideableValue<int>(Constants.STARTING_KARMA_POINTS, (v) => Constants.STARTING_KARMA_POINTS = v);

        public static void ResetStartingValues()
        {
            OVStartingKarma.Reset();
        }

        public static void ApplyOverrideValues()
        {
            if (!EnableOverrideStartingKarma) return;

            if (OverrideStartingKarma >= 0)
            {
                OVStartingKarma.Set(OverrideStartingKarma);
            }
        }

        [HarmonyPatch(typeof(Constants))]
        internal class ConstantsPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(Constants.LoadDefaults))]
            public static void LoadDefaultsPostfix()
            {
                // only applies if allowed
                ApplyOverrideValues();
            }
        }
    }
}
