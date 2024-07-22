using HarmonyLib;
using System;
using System.Collections.Generic;

namespace SRPlugin.Features.AlwaysBeSprinting
{
    public class AlwaysBeSprintingFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIAlwaysBeSprinting;

        public AlwaysBeSprintingFeature()
            : base(
                nameof(AlwaysBeSprinting),
                new List<ConfigItemBase>()
                {
                    (CIAlwaysBeSprinting = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(AlwaysBeSprinting), true, "makes some of the longer treks not so bad"))
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
            try
            {
                OVCombatSprint.Reset();
                OVCombatWalk.Reset();
                OVFriendlySprint.Reset();
                OVFriendlyWalk.Reset();
            }
            catch (Exception e)
            {
                SRPlugin.Squawk(e.ToString());
            }
        }

        public static bool AlwaysBeSprinting { get => CIAlwaysBeSprinting.GetValue(); set => CIAlwaysBeSprinting.SetValue(value); }

        private static OverrideableValue<int> OVCombatSprint = new OverrideableValue<int>(Constants.MOVE_THRESHOLD_COMBAT_SPRINT, (v) => Constants.MOVE_THRESHOLD_COMBAT_SPRINT = v, 0);
        private static OverrideableValue<int> OVCombatWalk = new OverrideableValue<int>(Constants.MOVE_THRESHOLD_COMBAT_WALK, (v) => Constants.MOVE_THRESHOLD_COMBAT_WALK = v, 0);
        private static OverrideableValue<int> OVFriendlySprint = new OverrideableValue<int>(Constants.MOVE_THRESHOLD_FRIENDLY_SPRINT, (v) => Constants.MOVE_THRESHOLD_FRIENDLY_SPRINT = v, 0);
        private static OverrideableValue<int> OVFriendlyWalk = new OverrideableValue<int>(Constants.MOVE_THRESHOLD_FRIENDLY_WALK, (v) => Constants.MOVE_THRESHOLD_FRIENDLY_WALK = v, 0);

        public static void ApplyOverrideValues()
        {
            try
            {
                if (!AlwaysBeSprinting) return;

                OVCombatSprint.SetDefault();
                OVCombatWalk.SetDefault();
                OVFriendlySprint.SetDefault();
                OVFriendlyWalk.SetDefault();
            }
            catch (Exception e)
            {
                SRPlugin.Squawk(e.ToString());
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
