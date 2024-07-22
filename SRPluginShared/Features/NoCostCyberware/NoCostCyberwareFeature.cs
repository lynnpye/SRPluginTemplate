using HarmonyLib;
using isogame;
using System.Collections.Generic;

namespace SRPlugin.Features.NoCostCyberware
{
    public class NoCostCyberwareFeature : FeatureImpl
    {
        private static ConfigItem<bool> CINoCostCyberware;

        public NoCostCyberwareFeature()
            : base(
                nameof(NoCostCyberware),
                new List<ConfigItemBase>()
                {
                    (CINoCostCyberware = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(NoCostCyberware), true, "overrides the call to GetDerivedEssence... essence-cost-free cyberware!")),
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

        [HarmonyPatch(typeof(Actor))]
        internal class ActorGetDerivedEssencePatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(Actor.GetDerivedEssence))]
            public static void GetDerivedEssence_Postfix(ref float __result, Actor __instance)
            {
                if (!NoCostCyberware) return;

                float newDerivedEssence = StatsUtil.GetAttributeMax(isogame.Attribute.Attribute_Magic_Essence);

#if SRHK
                int cyberwareAffinity = StatsUtil.GetSkill(__instance, Skill.Skill_CyberwareAffinity);
                newDerivedEssence += CyberwareAffinityEssenceBonusOverride.CyberwareAffinityEssenceBonusOverrideFeature.GetCyberwareAffinityBonusEssence(cyberwareAffinity);
#endif

                __result = newDerivedEssence;
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

                __result = 0f;
            }
        }
    }
}
