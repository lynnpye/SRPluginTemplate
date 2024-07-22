using HarmonyLib;
using System.Collections.Generic;

namespace SRPlugin.Features.OverrideStartingKarma
{
    internal class OverrideStartingKarmaFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIEnableOverrideStartingKarma;
        private static ConfigItem<int> CIOverrideStartingKarma;

        public OverrideStartingKarmaFeature()
            : base(
                nameof(EnableOverrideStartingKarma),
                new List<ConfigItemBase>()
                {
                    (CIEnableOverrideStartingKarma = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(EnableOverrideStartingKarma), true, "setting to false prevents patching")),
                    (CIOverrideStartingKarma = new ConfigItem<int>(nameof(OverrideStartingKarma), 60, "game default is 5; -1 also disables even with the feature enabled, but still patched"))
                }, new List<PatchRecord>(
                        PatchRecord.RecordPatches(
                            AccessTools.Method(typeof(ConstantsPatch), nameof(ConstantsPatch.LoadDefaultsPostfix))
                            )
                    )
                {
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
