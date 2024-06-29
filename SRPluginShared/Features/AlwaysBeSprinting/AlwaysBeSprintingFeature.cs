using HarmonyLib;
using SRPlugin;

namespace SRPlugin.Features.AlwaysBeSprinting
{
    [FeatureClass(FeatureEnum.AlwaysBeSprinting)]
    internal class AlwaysBeSprintingFeature : IFeature
    {
        private static int? ORIG_COMBAT_SPRINT = null;
        private static int? ORIG_COMBAT_WALK = null;
        private static int? ORIG_FRIENDLY_SPRINT = null;
        private static int? ORIG_FRIENDLY_WALK = null;

        public static void ResetAlwaysSprinting()
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

        public static void ApplyAlwaysSprinting()
        {
            if (FeatureConfig.AlwaysBeSprinting)
            {
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
        }

        public void ApplyPatches()
        {
            SRPlugin.Harmony.PatchAll(typeof(PlayerMotorPatch));
        }

        public void UnapplyPatches()
        {
            SRPlugin.Harmony.Unpatch(
                typeof(PlayerMotor).GetMethod(nameof(PlayerMotor.ComputeWalkMode)),
                typeof(PlayerMotorPatch).GetMethod(nameof(PlayerMotorPatch.ComputeWalkModePatch))
                );
        }

        [HarmonyPatch(typeof(PlayerMotor))]
        internal class PlayerMotorPatch
        {

            [HarmonyPrefix]
            [HarmonyPatch(nameof(PlayerMotor.ComputeWalkMode))]
            public static void ComputeWalkModePatch()
            {
                // only resets if changed
                ResetAlwaysSprinting();

                // only applies if allowed
                ApplyAlwaysSprinting();
            }
        }
    }
}
