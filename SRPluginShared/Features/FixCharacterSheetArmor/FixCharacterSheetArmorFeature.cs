using HarmonyLib;
using System.Collections.Generic;

namespace SRPlugin.Features.FixCharacterSheetArmor
{
    public class FixCharacterSheetArmorFeature : FeatureImpl
    {
#if SRR
        public FixCharacterSheetArmorFeature()
            : base(
                null,
                new List<ConfigItemBase>()
                {
                }, new List<PatchRecord>()
                {
                })
        {

        }
#else
        private static ConfigItem<bool> CIFixCharacterSheetArmor;

        public FixCharacterSheetArmorFeature()
            : base(
                nameof(FixCharacterSheetArmor),
                new List<ConfigItemBase>()
                {
                    (CIFixCharacterSheetArmor = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(FixCharacterSheetArmor), true, "tries to fix the character sheet not always displaying correct armor values")),
                }, new List<PatchRecord>()
                {
                    PatchRecord.Postfix(typeof(CharacterSheetWidget).GetMethod(nameof(CharacterSheetWidget.InitializeFromPlayer)),
                            typeof(CharacterSheetWidgetPatch).GetMethod(nameof(CharacterSheetWidgetPatch.InitializeFromPlayerPostfix))),

                    PatchRecord.Postfix(typeof(PDA).GetMethod(nameof(PDA.StartPDA)),
                            typeof(PDAPatch).GetMethod(nameof(PDAPatch.StartPDAPrefix))),
                    PatchRecord.Postfix(typeof(PDA).GetMethod(nameof(PDA.StartPDACharacter)),
                            typeof(PDAPatch).GetMethod(nameof(PDAPatch.StartPDAPrefix))),
                    PatchRecord.Postfix(typeof(PDA).GetMethod(nameof(PDA.CloseEquipScreen)),
                            typeof(PDAPatch).GetMethod(nameof(PDAPatch.CloseEquipScreenPostfix))),

                    PatchRecord.Postfix(typeof(CyberwareScreen).GetMethod(nameof(CyberwareScreen.Confirm)),
                        typeof(CyberwareScreenPatch).GetMethod(nameof(CyberwareScreenPatch.ConfirmPostfix))),
                    PatchRecord.Postfix(AccessTools.Method(typeof(StoreScreen), "Confirm"),
                        typeof(StoreScreenPatch).GetMethod(nameof(StoreScreenPatch.ConfirmPostfix))),
                })
        {

        }

        public static bool FixCharacterSheetArmor { get => CIFixCharacterSheetArmor.GetValue(); set => CIFixCharacterSheetArmor.SetValue(value); }

        public static bool UpdatePlayerRPOnNextEquipScreenClose { get; set; }

        [HarmonyPatch(typeof(CharacterSheetWidget))]
        internal class CharacterSheetWidgetPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(CharacterSheetWidget.InitializeFromPlayer))]
            public static void InitializeFromPlayerPostfix(CharacterSheetWidget __instance, Player player)
            {
                if (!(player == null))
                {
                    __instance.armorNumber.text = $"{player.RP} / {StatsUtil.GetMaxArmor(player)}";
                }
            }
        }

        [HarmonyPatch(typeof(PDA))]
        internal class PDAPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(PDA.StartPDA))]
            [HarmonyPatch(nameof(PDA.StartPDACharacter))]
            public static void StartPDAPrefix(PDA __instance)
            {
                __instance.RefreshCharacter();
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(PDA.CloseEquipScreen))]
            public static void CloseEquipScreenPostfix()
            {
                if (!UpdatePlayerRPOnNextEquipScreenClose) return;

                Player player = SceneSingletonBehavior<TurnDirector>.Instance.PlayerZero;

                if (!(player == null))
                {
                    player.SetRP(StatsUtil.GetMaxArmor(player));
                }
            }
        }

        [HarmonyPatch(typeof(CyberwareScreen))]
        internal class CyberwareScreenPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(CyberwareScreen.Confirm))]
            public static void ConfirmPostfix(Player ___thisPlayer)
            {
                if (!(___thisPlayer == null))
                {
                    ___thisPlayer.SetRP(StatsUtil.GetMaxArmor(___thisPlayer));
                }
            }
        }

        [HarmonyPatch(typeof(StoreScreen))]
        internal class StoreScreenPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("Confirm")]
            public static void ConfirmPostfix()
            {
                UpdatePlayerRPOnNextEquipScreenClose = true;
            }
        }
#endif
    }
}
