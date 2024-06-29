using HarmonyLib;
using SRPlugin;

namespace SRPlugin.Features.OverrideStartingKarma
{
    [FeatureClass(FeatureEnum.EnableOverrideStartingKarma)]
    internal class OverrideStartingKarmaFeature : IFeature
    {
        public static int? ORIGINAL_STARTING_KARMA_POINTS = null;

        public static void ResetStartingKarmaPoints()
        {
            if (ORIGINAL_STARTING_KARMA_POINTS.HasValue && ORIGINAL_STARTING_KARMA_POINTS.Value != Constants.STARTING_KARMA_POINTS)
            {
                Constants.STARTING_KARMA_POINTS = ORIGINAL_STARTING_KARMA_POINTS.Value;
            }
        }

        public static void ApplyStartingKarmaPointsOverride()
        {
            if (FeatureConfig.EnableOverrideStartingKarma && FeatureConfig.OverrideStartingKarma >= 0)
            {
                ORIGINAL_STARTING_KARMA_POINTS = ORIGINAL_STARTING_KARMA_POINTS ?? Constants.STARTING_KARMA_POINTS;
                Constants.STARTING_KARMA_POINTS = FeatureConfig.OverrideStartingKarma;
            }
        }

        public void ApplyPatches()
        {
            SRPlugin.Harmony.PatchAll(typeof(MainMenuScenePrefixPatch));
        }

        public void UnapplyPatches()
        {
            SRPlugin.Harmony.Unpatch(
                typeof(MainMenuScene).GetMethod(nameof(MainMenuScene.ShowNewGameScreen)),
                typeof(MainMenuScenePrefixPatch).GetMethod(nameof(MainMenuScenePrefixPatch.ShowNewGameScreen_Prefix))
                );
        }

        [HarmonyPatch(typeof(MainMenuScene))]
        internal class MainMenuScenePrefixPatch
        {

            [HarmonyPrefix]
            [HarmonyPriority(Priority.HigherThanNormal)]
            [HarmonyPatch(nameof(MainMenuScene.ShowNewGameScreen))]
            public static void ShowNewGameScreen_Prefix()
            {
                // only resets if changed
                ResetStartingKarmaPoints();

                // only applies if allowed
                ApplyStartingKarmaPointsOverride();
            }
        }
    }
}
