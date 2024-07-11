using HarmonyLib;
using Localize;
using System.Collections;
using System.Collections.Generic;

namespace SRPlugin.Features.ShopQoL
{
    public class ShopQoLFeature : FeatureImpl
    {
#if SRHK
        public ShopQoLFeature() : base(null, null, null) {}
#else
        private static ConfigItem<bool> CIShopQoL;

        public ShopQoLFeature()
            : base(
                nameof(ShopQoL),
                new List<ConfigItemBase>()
                {
                    (CIShopQoL = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(ShopQoL), true, "adds some QoL changes to the store experience")),
                },
                new List<PatchRecord>()
                {
                    PatchRecord.Postfix(
                        typeof(StoreScreen).GetMethod(nameof(StoreScreen.Initialize)),
                        typeof(StoreScreenPatch).GetMethod(nameof(StoreScreenPatch.InitializePostfix))
                        ),
                    PatchRecord.Postfix(
                        AccessTools.Method(typeof(EquipScreen), "SetEquipmentFilters"),
                        typeof(EquipScreenPatch).GetMethod(nameof(EquipScreenPatch.SetEquipmentFiltersPostfix))
                        ),
                })
        {
        }

        public static bool ShopQoL { get => CIShopQoL.GetValue(); set => CIShopQoL.SetValue(value); }

        [HarmonyPatch(typeof(StoreScreen))]
        internal class StoreScreenPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(StoreScreen.Initialize))]
            public static IEnumerator InitializePostfix(IEnumerator __result, StoreScreen __instance, BetterList<StoreTab> ___activeTabList)
            {
                // run original enumerator code
                while (__result.MoveNext())
                {
                    yield return __result.Current;
                }

                // now do our stuff
                if (___activeTabList == null || ___activeTabList.size < 2 || !___activeTabList[0].backgroundSelected.gameObject.activeSelf)
                {
                    yield break;
                }

                // arbitrarily pick the second tab if possible
                __instance.OnStoreTabClicked(___activeTabList[1].button);
            }
        }

        [HarmonyPatch(typeof(EquipScreen))]
        internal class EquipScreenPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(EquipScreen), "SetEquipmentFilters")]
            public static void SetEquipmentFiltersPostfix(EquipScreen __instance, BetterList<BasicButton> ___equipFilterList)
            {
                if (__instance == null || ___equipFilterList == null || ___equipFilterList.size < 2) return;

                var button0 = ___equipFilterList[0];
                if (!string.Equals(button0?.label?.text, Strings.T("All"))) return;

                AccessTools.Method(typeof(EquipScreen), "SetActiveFilterButton").Invoke(__instance, [___equipFilterList[1]]);
            }
        }
#endif
    }
}
