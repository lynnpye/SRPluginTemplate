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
                typeof(ConstantsPatch).GetMethod(nameof(ConstantsPatch.LoadDefaultsPostfix))
                );

            ResetStartingValues();
        }

        public static int OverrideStartingKarma
        {
            get
            {
                return CIOverrideStartingKarma.GetValue();
            }

            set
            {
                CIOverrideStartingKarma.SetValue(value);
            }
        }

        public static bool EnableOverrideStartingKarma
        {
            get
            {
                return CIEnableOverrideStartingKarma.GetValue();
            }

            set
            {
                CIEnableOverrideStartingKarma.SetValue(value);
            }
        }

        private static int? ORIGINAL_STARTING_KARMA_POINTS = null;

        public static void ResetStartingValues()
        {
            if (ORIGINAL_STARTING_KARMA_POINTS.HasValue)
            {
                Constants.STARTING_KARMA_POINTS = ORIGINAL_STARTING_KARMA_POINTS.Value;
                ORIGINAL_STARTING_KARMA_POINTS = null;
            }
        }

        public static void ApplyOverrideValues()
        {
            if (!EnableOverrideStartingKarma) return;

            if (OverrideStartingKarma >= 0)
            {
                if (!ORIGINAL_STARTING_KARMA_POINTS.HasValue)
                {
                    ORIGINAL_STARTING_KARMA_POINTS = Constants.STARTING_KARMA_POINTS;
                }
                Constants.STARTING_KARMA_POINTS = OverrideStartingKarma;
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
