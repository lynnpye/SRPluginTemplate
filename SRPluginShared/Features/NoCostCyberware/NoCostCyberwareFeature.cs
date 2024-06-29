using HarmonyLib;
using SRPlugin;

namespace SRPlugin.Features.NoCostCyberware
{
    [FeatureClass(FeatureEnum.NoCostCyberware)]
    internal class NoCostCyberwareFeature : IFeature
    {
        public void UnapplyPatches()
        {
            SRPlugin.Harmony.Unpatch(
                typeof(Actor).GetMethod(nameof(Actor.GetDerivedEssence)),
                typeof(ActorGetDerivedEssencePatch).GetMethod(nameof(ActorGetDerivedEssencePatch.GetDerivedEssence_Postfix))
                );
        }

        public void ApplyPatches()
        {
            SRPlugin.Harmony.PatchAll(typeof(ActorGetDerivedEssencePatch));
        }

        [HarmonyPatch(typeof(Actor))]
        internal class ActorGetDerivedEssencePatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(Actor.GetDerivedEssence))]
            public static void GetDerivedEssence_Postfix(ref float __result, Actor __instance)
            {
                if (!FeatureConfig.NoCostCyberware) return;

                __result = StatsUtil.GetAttributeMax(isogame.Attribute.Attribute_Magic_Essence);
            }
        }
    }
}
