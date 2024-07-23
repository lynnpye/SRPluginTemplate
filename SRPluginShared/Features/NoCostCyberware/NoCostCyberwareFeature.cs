using HarmonyLib;
using isogame;
using System.Collections.Generic;

namespace SRPlugin.Features.NoCostCyberware
{
    public class NoCostCyberwareFeature : FeatureImpl
    {
        private static ConfigItem<bool> CINoCostCyberware;
        private static ConfigItem<float> CICyberwareCostMultiplier;

        public NoCostCyberwareFeature()
            : base(
                nameof(NoCostCyberware),
                new List<ConfigItemBase>()
                {
                    (CINoCostCyberware = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(NoCostCyberware), true,
$@"
overrides the call to GetDerivedEssence... multiply the calculated cyberware cost by a float,
0 to remove all essence cost, 0.5 for half cost, 1 for standard play, >1 for increased challenge.
no actual limit though, so have fun with negative essence costs cyberware
(i.e. more ware more essence?) or essence costs increased above normal
(maybe with a content pack that adds high power cyberware)
"                       
                        
                        )),
                    (CICyberwareCostMultiplier = new ConfigItem<float>(nameof(CyberwareCostMultiplier), 0f, "multiply cyberware essence cost by this value; 0 to remove all essence cost, 0.5 for half cost, 1 for standard play, >1 for increased challenge")),
                }, new List<PatchRecord>(
                        PatchRecord.RecordPatches(
                            AccessTools.Method(typeof(ActorGetDerivedEssencePatch), nameof(ActorGetDerivedEssencePatch.GetDerivedEssence_Postfix)),
                            AccessTools.Method(typeof(CyberwareScreenPatch), nameof(CyberwareScreenPatch.GetEssenceLostFromItemDefPrefix))
                            
                            )
                    )
                {
                })
        {

        }

        public static bool NoCostCyberware { get => CINoCostCyberware.GetValue(); set => CINoCostCyberware.SetValue(value); }
        public static float CyberwareCostMultiplier { get => CICyberwareCostMultiplier.GetValue(); set => CICyberwareCostMultiplier.SetValue(value); }

        [HarmonyPatch(typeof(Actor))]
        internal class ActorGetDerivedEssencePatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(Actor.GetDerivedEssence))]
            public static void GetDerivedEssence_Postfix(ref float __result, Actor __instance)
            {
                if (!NoCostCyberware) return;

                float derivedEssenceAdjustment = 0f;
                for (int i = 0; i < __instance.activeEffects.Count; i++)
                {
                    StatusEffects statusEffects = __instance.activeEffects[i];
                    for (int j = 0; j < statusEffects.statMods.Count; j++)
                    {
                        StatMod statMod = statusEffects.statMods[j];
                        if (statMod.attribute == global::isogame.Attribute.Attribute_Magic_Essence)
                        {
                            derivedEssenceAdjustment += statMod.floatModValue * (1f - CyberwareCostMultiplier);
                        }
                    }
                }

                // so instead of overwriting, we are adjusting by the difference between what was originally
                // calculated and what we think should have been calculated
                // this avoids having to worry about the bonuses added by cyberware affinity
                __result = __result - derivedEssenceAdjustment;
            }
        }

        [HarmonyPatch(typeof(CyberwareScreen))]
        internal class CyberwareScreenPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(CyberwareScreen.GetEssenceLostFromItemDef))]
            public static void GetEssenceLostFromItemDefPrefix(ref float __result)
            {
                if (!NoCostCyberware) return;

                __result = __result * CyberwareCostMultiplier;
            }
        }
    }
}
