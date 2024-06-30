using HarmonyLib;
using System.Collections.Generic;

namespace SRPlugin.Features.AlwaysBeSprinting
{
    public class AlwaysBeSprintingFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIAlwaysBeSprinting;

        public AlwaysBeSprintingFeature()
            : base(new List<ConfigItemBase>()
                {
                    (CIAlwaysBeSprinting = new ConfigItem<bool>(FEATURES_SECTION, nameof(AlwaysBeSprinting), true, "makes some of the longer treks not so bad"))
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
        }

        public static bool AlwaysBeSprinting
        {
            get
            {
                return CIAlwaysBeSprinting.GetValue();
            }

            set
            {
                CIAlwaysBeSprinting.SetValue(value);
            }
        }

        private static int? ORIG_COMBAT_SPRINT = null;
        private static int? ORIG_COMBAT_WALK = null;
        private static int? ORIG_FRIENDLY_SPRINT = null;
        private static int? ORIG_FRIENDLY_WALK = null;

        public static void ResetStartingValues()
        {
            if (ORIG_COMBAT_SPRINT.HasValue)
            {
                Constants.MOVE_THRESHOLD_COMBAT_SPRINT = ORIG_COMBAT_SPRINT.Value;
                Constants.MOVE_THRESHOLD_COMBAT_WALK = ORIG_COMBAT_WALK.Value;
                Constants.MOVE_THRESHOLD_FRIENDLY_SPRINT = ORIG_FRIENDLY_SPRINT.Value;
                Constants.MOVE_THRESHOLD_FRIENDLY_WALK = ORIG_FRIENDLY_WALK.Value;

                ORIG_COMBAT_SPRINT = null;
                ORIG_COMBAT_WALK = null;
                ORIG_FRIENDLY_SPRINT = null;
                ORIG_FRIENDLY_WALK = null;
            }
        }

        public static void ApplyOverrideValues()
        {
            if (!AlwaysBeSprinting) return;

            if (!ORIG_COMBAT_SPRINT.HasValue)
            {
                ORIG_COMBAT_SPRINT = Constants.MOVE_THRESHOLD_COMBAT_SPRINT;
                ORIG_COMBAT_WALK = Constants.MOVE_THRESHOLD_COMBAT_WALK;
                ORIG_FRIENDLY_SPRINT = Constants.MOVE_THRESHOLD_FRIENDLY_SPRINT;
                ORIG_FRIENDLY_WALK = Constants.MOVE_THRESHOLD_FRIENDLY_WALK;

                Constants.MOVE_THRESHOLD_COMBAT_SPRINT = 0;
                Constants.MOVE_THRESHOLD_COMBAT_WALK = 0;
                Constants.MOVE_THRESHOLD_FRIENDLY_SPRINT = 0;
                Constants.MOVE_THRESHOLD_FRIENDLY_WALK = 0;
            }
        }

        [HarmonyPatch(typeof(Constants))]
        internal class ConstantsPatch
        {

            [HarmonyPostfix]
            [HarmonyPatch(nameof(Constants.LoadDefaults))]
            public static void LoadDefaultsPatch()
            {
                // only applies if allowed
                ApplyOverrideValues();
            }
        }
    }
}
