using HarmonyLib;
using isogame;
using System;
using System.Collections.Generic;

namespace SRPlugin.Features.NoCostCyberware
{
    public class NoCostCyberwareFeature : FeatureImpl
    {
        private static ConfigItem<bool> CINoCostCyberware;

        public NoCostCyberwareFeature()
            : base(new List<ConfigItemBase>()
            {
                (CINoCostCyberware = new ConfigItem<bool>(FEATURES_SECTION, nameof(NoCostCyberware), true, "overrides the call to GetDerivedEssence... essence-cost-free cyberware!"))
            })
        {

        }

        public override void HandleDisabled()
        {
            SRPlugin.Harmony.Unpatch(
                typeof(Actor).GetMethod(nameof(Actor.GetDerivedEssence)),
                typeof(ActorGetDerivedEssencePatch).GetMethod(nameof(ActorGetDerivedEssencePatch.GetDerivedEssence_Postfix))
                );
            
            SRPlugin.Harmony.Unpatch(
                typeof(CyberwareScreen).GetMethod(nameof(CyberwareScreen.GetEssenceLostFromItemDef)),
                typeof(CyberwareScreenPatch).GetMethod(nameof(CyberwareScreenPatch.GetEssenceLostFromItemDefPrefix))
                );
        }

        public override void HandleEnabled()
        {
            SRPlugin.Harmony.PatchAll(typeof(ActorGetDerivedEssencePatch));
            SRPlugin.Harmony.PatchAll(typeof(CyberwareScreenPatch));
        }

        public static bool NoCostCyberware
        {
            get
            {
                return CINoCostCyberware.GetValue();
            }

            set
            {
                CINoCostCyberware.SetValue(value);
            }
        }

        [HarmonyPatch(typeof(Actor))]
        internal class ActorGetDerivedEssencePatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(Actor.GetDerivedEssence))]
            public static void GetDerivedEssence_Postfix(ref float __result)
            {
                if (!NoCostCyberware) return;

                __result = Math.Max(__result, StatsUtil.GetAttributeMax(isogame.Attribute.Attribute_Magic_Essence));
            }
        }

        [HarmonyPatch(typeof(CyberwareScreen))]
        internal class CyberwareScreenPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(CyberwareScreen.GetEssenceLostFromItemDef))]
            public static bool GetEssenceLostFromItemDefPrefix(ref float __result)
            {
                __result = 0f;
                return false;
            }
        }
    }
}
