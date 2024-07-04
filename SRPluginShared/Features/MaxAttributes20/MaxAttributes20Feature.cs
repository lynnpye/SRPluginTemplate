using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace SRPlugin.Features.MaxAttributes20
{
    internal class MaxAttributes20Feature : FeatureImpl
    {
        private static ConfigItem<bool> CIMaxAttributes20;
#if NARROWKARMABUTTONS
        private static ConfigItem<bool> CINarrowKarmaButton;
        private static ConfigItem<bool> CISimulatedClickLastPossible;
#endif

        public MaxAttributes20Feature()
            : base(new List<ConfigItemBase>()
            {
                (CIMaxAttributes20 = new ConfigItem<bool>(FEATURES_SECTION, nameof(MaxAttributes20), true, "20 fits nicely, you should see my fix for DF lol")),
#if NARROWKARMABUTTONS
                (CINarrowKarmaButton = new ConfigItem<bool>(FEATURES_SECTION, nameof(NarrowKarmaButtons), true, "narrows the karma UI to fit a max value of 20")),
                (CISimulatedClickLastPossible = new ConfigItem<bool>(FEATURES_SECTION, nameof(SimulatedClickLastPossible), true, "clicking the last value in a karma row simulates clicking the last available, simulates SRHK functionality"))
#endif
            }, new List<PatchRecord>()
            {
                PatchRecord.Postfix(
                    typeof(StatsUtil).GetMethod(nameof(StatsUtil.IsNewEtiquetteLevel)),
                    typeof(StatsUtilGetAttributeMaxPatch).GetMethod(nameof(StatsUtilGetAttributeMaxPatch.IsNewEtiquetteLevelPostfix))
                    ),
                PatchRecord.Postfix(
                    typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax)),
                    typeof(StatsUtilGetAttributeMaxPatch).GetMethod(nameof(StatsUtilGetAttributeMaxPatch.GetAttributeMax_Postfix))
                    ),
                PatchRecord.Postfix(
                    typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax_Dwarf)),
                    typeof(StatsUtilGetAttributeMaxPatch).GetMethod(nameof(StatsUtilGetAttributeMaxPatch.GetAttributeMax_Dwarf_Postfix))
                    ),
                PatchRecord.Postfix(
                    typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax_Elf)),
                    typeof(StatsUtilGetAttributeMaxPatch).GetMethod(nameof(StatsUtilGetAttributeMaxPatch.GetAttributeMax_Elf_Postfix))
                    ),
                PatchRecord.Postfix(
                    typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax_Human)),
                    typeof(StatsUtilGetAttributeMaxPatch).GetMethod(nameof(StatsUtilGetAttributeMaxPatch.GetAttributeMax_Human_Postfix))
                    ),
                PatchRecord.Postfix(
                    typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax_Ork)),
                    typeof(StatsUtilGetAttributeMaxPatch).GetMethod(nameof(StatsUtilGetAttributeMaxPatch.GetAttributeMax_Ork_Postfix))
                    ),
                PatchRecord.Postfix(
                    typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax_Troll)),
                    typeof(StatsUtilGetAttributeMaxPatch).GetMethod(nameof(StatsUtilGetAttributeMaxPatch.GetAttributeMax_Troll_Postfix))
                    ),
#if NARROWKARMABUTTONS
                PatchRecord.Prefix(
                    typeof(KarmaEntry2).GetMethod(nameof(KarmaEntry2.OnBlockClick)),
                    typeof(KarmaEntry2SimulatedClickLastPossiblePatch).GetMethod(nameof(KarmaEntry2SimulatedClickLastPossiblePatch.OnBlockClickPrefix))
                    ),
                PatchRecord.Postfix(
                    typeof(KarmaEntry2).GetMethod(nameof(KarmaEntry2.Refresh)),
                    typeof(KarmaEntry2SimulatedClickLastPossiblePatch).GetMethod(nameof(KarmaEntry2SimulatedClickLastPossiblePatch.RefreshPostfix))
                    ),
                PatchRecord.Postfix(
                    typeof(KarmaEntry2).GetMethod(nameof(KarmaEntry2.Initialize)),
                    typeof(KarmaEntry2SimulatedClickLastPossiblePatch).GetMethod(nameof(KarmaEntry2SimulatedClickLastPossiblePatch.InitializePostfix))
                    ),
#endif
            })
        {

        }


        public static bool MaxAttributes20 { get => CIMaxAttributes20.GetValue(); set => CIMaxAttributes20.SetValue(value); }
#if NARROWKARMABUTTONS
        public static bool NarrowKarmaButtons { get => CINarrowKarmaButton.GetValue(); set => CINarrowKarmaButton.SetValue(value); }
        public static bool SimulatedClickLastPossible { get => CISimulatedClickLastPossible.GetValue(); set => CISimulatedClickLastPossible.SetValue(value); }
#endif

#if NARROWKARMABUTTONS
        public static float? ORIGINAL_BLOCK_WIDTH = null;
        public static float? ORIGINAL_BLOCK_HALF_WIDTH = null;

        public static float NARROW_BLOCK_WIDTH = 40f;

        public static float SCALE_MULTIPLIER
        {
            get
            {
                if (!MaxAttributes20 || !NarrowKarmaButtons) return 1f;

                return NARROW_BLOCK_WIDTH / (ORIGINAL_BLOCK_WIDTH ?? KarmaBlock.BLOCK_WIDTH);
            }
        }
#endif

        /*
         * This patch is provided as a proof of concept so you can easily see that the plugin, once built and installed, works.
         */
        [HarmonyPatch(typeof(StatsUtil))]
        internal class StatsUtilGetAttributeMaxPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.IsNewEtiquetteLevel))]
            public static void IsNewEtiquetteLevelPostfix(ref bool __result, int charisma)
            {
                if (charisma > (2 * Constants.NUM_ETIQUETTES))
                {
                    __result = false;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax))]
            public static void GetAttributeMax_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributes20) return;

                switch (entry)
                {
                    case isogame.Attribute.Attribute_Body:
                    case isogame.Attribute.Attribute_Quickness:
                    case isogame.Attribute.Attribute_Strength:
                    case isogame.Attribute.Attribute_Charisma:
                    case isogame.Attribute.Attribute_Intelligence:
                    case isogame.Attribute.Attribute_Willpower:
                        __result = 20;
                        break;
                }
            }

            private static void GetAttributeMax_Racial(ref int __result, isogame.Attribute entry)
            {
                switch (entry)
                {
                    case isogame.Attribute.Attribute_Body:
                    case isogame.Attribute.Attribute_Quickness:
                    case isogame.Attribute.Attribute_Strength:
                    case isogame.Attribute.Attribute_Charisma:
                    case isogame.Attribute.Attribute_Intelligence:
                    case isogame.Attribute.Attribute_Willpower:
                        __result = StatsUtil.GetAttributeMax(entry); // play by the rules instead of bypassing
                        break;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax_Dwarf))]
            public static void GetAttributeMax_Dwarf_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributes20) return;

                GetAttributeMax_Racial(ref __result, entry);
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax_Elf))]
            public static void GetAttributeMax_Elf_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributes20) return;

                GetAttributeMax_Racial(ref __result, entry);
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax_Human))]
            public static void GetAttributeMax_Human_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributes20) return;

                GetAttributeMax_Racial(ref __result, entry);
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax_Ork))]
            public static void GetAttributeMax_Ork_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributes20) return;

                GetAttributeMax_Racial(ref __result, entry);
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax_Troll))]
            public static void GetAttributeMax_Troll_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributes20) return;

                GetAttributeMax_Racial(ref __result, entry);
            }
        }


#if NARROWKARMABUTTONS
        [HarmonyPatch(typeof(KarmaEntry2))]
        internal class KarmaEntry2SimulatedClickLastPossiblePatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(KarmaEntry2.OnBlockClick))]
            public static bool OnBlockClickPrefix(KarmaEntry2 __instance, ref KarmaBlock block, string button)
            {
                if (!MaxAttributes20 || !SimulatedClickLastPossible) return true;

                BetterList<KarmaBlock> blockList = PrivateEye.GetPrivateFieldValue<BetterList<KarmaBlock>>(__instance, "blockList", null);
                if (blockList == null)
                {
                    // not sure what happened but let normal code try to handle it
                    return true;
                }

                KarmaScreen2 parentKarmaScreen = PrivateEye.GetPrivateFieldValue<KarmaScreen2>(__instance, "parentKarmaScreen", null);
                if (!SimulatedClickLastPossible || button != "button" || parentKarmaScreen == null || block.index + 1 < blockList.size)
                {
                    // disabled, do nothing and let normal code run
                    return true;
                }

                KarmaBlock newBlock = blockList[0];
                while (newBlock.index + 1 < blockList.size && (newBlock.newAllocated || newBlock.allocated))
                {
                    newBlock = blockList[newBlock.index + 1];
                }

                block = newBlock;

                // nope, you got it
                return true;
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(KarmaEntry2.Refresh))]
            public static void RefreshPostfix(KarmaEntry2 __instance)
            {
                if (!MaxAttributes20 || !SimulatedClickLastPossible) return;

                BetterList<KarmaBlock> blockList = PrivateEye.GetPrivateFieldValue<BetterList<KarmaBlock>>(__instance, "blockList", null);
                if (blockList == null || blockList.size < 1)
                {
                    // not sure what happened but let normal code try to handle it
                    return;
                }

                KarmaBlock block = blockList[blockList.size - 1];
                block.buttonCol.enabled = true;
                block.SetOverlayOn();
                block.activeFrame.enabled = true;
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(KarmaEntry2.Initialize))]
            public static void InitializePostfix(KarmaEntry2 __instance)
            {
                if (!MaxAttributes20 || !NarrowKarmaButtons) return;

                __instance.transform.localScale = new Vector3(SCALE_MULTIPLIER, 1f, 1f);
            }
        }
#endif
    }
}
