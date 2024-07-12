using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRPlugin.Features.ExtraWeaponSlot
{
    public class ExtraWeaponSlotFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIExtraWeaponSlot;

        public ExtraWeaponSlotFeature()
            : base(
                nameof(ExtraWeaponSlot),
                new List<ConfigItemBase>()
                {
                    (CIExtraWeaponSlot = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(ExtraWeaponSlot), true, "adds 1 extra weapon slot")),
                },
                new List<PatchRecord>()
                {
                    PatchRecord.Postfix(
                        typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetMaxWeaponSlots)),
                        typeof(StatsUtilPatch).GetMethod(nameof(StatsUtilPatch.GetMaxWeaponSlotsPostfix))
                        ),
                })
        {

        }

        public static bool ExtraWeaponSlot { get => CIExtraWeaponSlot.GetValue(); set => CIExtraWeaponSlot.SetValue(value); }

        [HarmonyPatch(typeof(StatsUtil))]
        internal class StatsUtilPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetMaxWeaponSlots))]
            public static void GetMaxWeaponSlotsPostfix(ref int __result, Player player)
            {
                if (!ExtraWeaponSlot) return;

                __result++;
            }
        }
    }
}
