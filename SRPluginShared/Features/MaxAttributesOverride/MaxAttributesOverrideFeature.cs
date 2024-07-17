using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRPlugin.Features.MaxAttributesOverride
{
    internal class MaxAttributesOverrideFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIMaxAttributesOverride;
        private static ConfigItem<int> CIResetAllAttributeMaxesToValue;
#if NARROWKARMABUTTONS
        private static ConfigItem<bool> CINarrowKarmaButton;
        private static ConfigItem<bool> CISimulatedClickLastPossible;
#endif

        // DwarfMaxes
        private static ConfigItem<int> CIMaxDwarfBOD;
        private static ConfigItem<int> CIMaxDwarfQUI;
        private static ConfigItem<int> CIMaxDwarfSTR;
        private static ConfigItem<int> CIMaxDwarfCHA;
        private static ConfigItem<int> CIMaxDwarfINT;
        private static ConfigItem<int> CIMaxDwarfWIL;

        // ElfMaxes
        private static ConfigItem<int> CIMaxElfBOD;
        private static ConfigItem<int> CIMaxElfQUI;
        private static ConfigItem<int> CIMaxElfSTR;
        private static ConfigItem<int> CIMaxElfCHA;
        private static ConfigItem<int> CIMaxElfINT;
        private static ConfigItem<int> CIMaxElfWIL;

        // HumanMaxes
        private static ConfigItem<int> CIMaxHumanBOD;
        private static ConfigItem<int> CIMaxHumanQUI;
        private static ConfigItem<int> CIMaxHumanSTR;
        private static ConfigItem<int> CIMaxHumanCHA;
        private static ConfigItem<int> CIMaxHumanINT;
        private static ConfigItem<int> CIMaxHumanWIL;

        // OrkMaxes
        private static ConfigItem<int> CIMaxOrkBOD;
        private static ConfigItem<int> CIMaxOrkQUI;
        private static ConfigItem<int> CIMaxOrkSTR;
        private static ConfigItem<int> CIMaxOrkCHA;
        private static ConfigItem<int> CIMaxOrkINT;
        private static ConfigItem<int> CIMaxOrkWIL;

        // TrollMaxes
        private static ConfigItem<int> CIMaxTrollBOD;
        private static ConfigItem<int> CIMaxTrollQUI;
        private static ConfigItem<int> CIMaxTrollSTR;
        private static ConfigItem<int> CIMaxTrollCHA;
        private static ConfigItem<int> CIMaxTrollINT;
        private static ConfigItem<int> CIMaxTrollWIL;


        public static string ATTRIBUTES_SECTION = "Attributes";

        public MaxAttributesOverrideFeature()
            : base(
                nameof(MaxAttributesOverride),
                new List<ConfigItemBase>()
                {
                    (CIMaxAttributesOverride = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(MaxAttributesOverride), true, "true - set the override values you want for each item, cap of 20, can be lower than game defaults ; false - disables/ignores everything in the [Attributes] section")),
                    (CIResetAllAttributeMaxesToValue = new ConfigItem<int>(PLUGIN_FEATURES_SECTION, nameof(ResetAllAttributeMaxesToValue), 0, "1 to 20 - resets all max attributes to specified on next launch, then sets itself to 0 ; 0 or less than 0 or greater than 20 - normalizes to 0, does nothing")),
#if NARROWKARMABUTTONS
                    (CISimulatedClickLastPossible = new ConfigItem<bool>(nameof(SimulatedClickLastPossible), true, "clicking the last value in a karma row simulates clicking the last available, simulates SRHK functionality")),
                    (CINarrowKarmaButton = new ConfigItem<bool>(nameof(NarrowKarmaButtons), true, "narrows the karma UI if any attribute max value is in range [18 , 20], has no effect otherwise (should probably leave true)")),
#endif
                    // MaxDwarf
                    (CIMaxDwarfBOD = new ConfigItem<int>(nameof(CIMaxDwarfBOD), 20, "max BOD for Dwarfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxDwarfQUI = new ConfigItem<int>(nameof(CIMaxDwarfQUI), 20, "max QUI for Dwarfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxDwarfSTR = new ConfigItem<int>(nameof(CIMaxDwarfSTR), 20, "max STR for Dwarfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxDwarfINT = new ConfigItem<int>(nameof(CIMaxDwarfINT), 20, "max INT for Dwarfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxDwarfWIL = new ConfigItem<int>(nameof(CIMaxDwarfWIL), 20, "max WIL for Dwarfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxDwarfCHA = new ConfigItem<int>(nameof(CIMaxDwarfCHA), 20, "max CHA for Dwarfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    // MaxElf
                    (CIMaxElfBOD = new ConfigItem<int>(nameof(CIMaxElfBOD), 20, "max BOD for Elfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxElfQUI = new ConfigItem<int>(nameof(CIMaxElfQUI), 20, "max QUI for Elfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxElfSTR = new ConfigItem<int>(nameof(CIMaxElfSTR), 20, "max STR for Elfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxElfINT = new ConfigItem<int>(nameof(CIMaxElfINT), 20, "max INT for Elfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxElfWIL = new ConfigItem<int>(nameof(CIMaxElfWIL), 20, "max WIL for Elfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxElfCHA = new ConfigItem<int>(nameof(CIMaxElfCHA), 20, "max CHA for Elfs, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    // MaxHuman
                    (CIMaxHumanBOD = new ConfigItem<int>(nameof(CIMaxHumanBOD), 20, "max BOD for Humans, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxHumanQUI = new ConfigItem<int>(nameof(CIMaxHumanQUI), 20, "max QUI for Humans, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxHumanSTR = new ConfigItem<int>(nameof(CIMaxHumanSTR), 20, "max STR for Humans, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxHumanINT = new ConfigItem<int>(nameof(CIMaxHumanINT), 20, "max INT for Humans, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxHumanWIL = new ConfigItem<int>(nameof(CIMaxHumanWIL), 20, "max WIL for Humans, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxHumanCHA = new ConfigItem<int>(nameof(CIMaxHumanCHA), 20, "max CHA for Humans, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    // MaxOrk
                    (CIMaxOrkBOD = new ConfigItem<int>(nameof(CIMaxOrkBOD), 20, "max BOD for Orks, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxOrkQUI = new ConfigItem<int>(nameof(CIMaxOrkQUI), 20, "max QUI for Orks, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxOrkSTR = new ConfigItem<int>(nameof(CIMaxOrkSTR), 20, "max STR for Orks, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxOrkINT = new ConfigItem<int>(nameof(CIMaxOrkINT), 20, "max INT for Orks, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxOrkWIL = new ConfigItem<int>(nameof(CIMaxOrkWIL), 20, "max WIL for Orks, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxOrkCHA = new ConfigItem<int>(nameof(CIMaxOrkCHA), 20, "max CHA for Orks, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    // MaxTroll
                    (CIMaxTrollBOD = new ConfigItem<int>(nameof(CIMaxTrollBOD), 20, "max BOD for Trolls, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxTrollQUI = new ConfigItem<int>(nameof(CIMaxTrollQUI), 20, "max QUI for Trolls, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxTrollSTR = new ConfigItem<int>(nameof(CIMaxTrollSTR), 20, "max STR for Trolls, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxTrollINT = new ConfigItem<int>(nameof(CIMaxTrollINT), 20, "max INT for Trolls, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxTrollWIL = new ConfigItem<int>(nameof(CIMaxTrollWIL), 20, "max WIL for Trolls, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                    (CIMaxTrollCHA = new ConfigItem<int>(nameof(CIMaxTrollCHA), 20, "max CHA for Trolls, set to 0 or less to disable (defaults to whatever the value is without this patch)")),
                }, new List<PatchRecord>()
                {
                    PatchRecord.Postfix(
                        typeof(StatsUtil).GetMethod(nameof(StatsUtil.IsNewEtiquetteLevel)),
                        typeof(StatsUtilPatch).GetMethod(nameof(StatsUtilPatch.IsNewEtiquetteLevelPostfix))
                        ),
                    PatchRecord.Postfix(
                        typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax)),
                        typeof(StatsUtilPatch).GetMethod(nameof(StatsUtilPatch.GetAttributeMax_Postfix))
                        ),
                    PatchRecord.Postfix(
                        typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax_Dwarf)),
                        typeof(StatsUtilPatch).GetMethod(nameof(StatsUtilPatch.GetAttributeMax_Dwarf_Postfix))
                        ),
                    PatchRecord.Postfix(
                        typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax_Elf)),
                        typeof(StatsUtilPatch).GetMethod(nameof(StatsUtilPatch.GetAttributeMax_Elf_Postfix))
                        ),
                    PatchRecord.Postfix(
                        typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax_Human)),
                        typeof(StatsUtilPatch).GetMethod(nameof(StatsUtilPatch.GetAttributeMax_Human_Postfix))
                        ),
                    PatchRecord.Postfix(
                        typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax_Ork)),
                        typeof(StatsUtilPatch).GetMethod(nameof(StatsUtilPatch.GetAttributeMax_Ork_Postfix))
                        ),
                    PatchRecord.Postfix(
                        typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetAttributeMax_Troll)),
                        typeof(StatsUtilPatch).GetMethod(nameof(StatsUtilPatch.GetAttributeMax_Troll_Postfix))
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


        public static bool MaxAttributesOverride { get => CIMaxAttributesOverride.GetValue(); set => CIMaxAttributesOverride.SetValue(value); }
        public static int ResetAllAttributeMaxesToValue { get => CIResetAllAttributeMaxesToValue.GetValue(); set => CIResetAllAttributeMaxesToValue.SetValue(value); }
#if NARROWKARMABUTTONS
        public static bool NarrowKarmaButtons { get => CINarrowKarmaButton.GetValue(); set => CINarrowKarmaButton.SetValue(value); }
        public static bool SimulatedClickLastPossible { get => CISimulatedClickLastPossible.GetValue(); set => CISimulatedClickLastPossible.SetValue(value); }
#endif

        // Max Dwarf
        public static int MaxDwarfBOD { get => CIMaxDwarfBOD.GetValue(); set => CIMaxDwarfBOD.SetValue(value); }
        public static int MaxDwarfQUI { get => CIMaxDwarfQUI.GetValue(); set => CIMaxDwarfQUI.SetValue(value); }
        public static int MaxDwarfSTR { get => CIMaxDwarfSTR.GetValue(); set => CIMaxDwarfSTR.SetValue(value); }
        public static int MaxDwarfCHA { get => CIMaxDwarfCHA.GetValue(); set => CIMaxDwarfCHA.SetValue(value); }
        public static int MaxDwarfINT { get => CIMaxDwarfINT.GetValue(); set => CIMaxDwarfINT.SetValue(value); }
        public static int MaxDwarfWIL { get => CIMaxDwarfWIL.GetValue(); set => CIMaxDwarfWIL.SetValue(value); }

        // Max Elf
        public static int MaxElfBOD { get => CIMaxElfBOD.GetValue(); set => CIMaxElfBOD.SetValue(value); }
        public static int MaxElfQUI { get => CIMaxElfQUI.GetValue(); set => CIMaxElfQUI.SetValue(value); }
        public static int MaxElfSTR { get => CIMaxElfSTR.GetValue(); set => CIMaxElfSTR.SetValue(value); }
        public static int MaxElfCHA { get => CIMaxElfCHA.GetValue(); set => CIMaxElfCHA.SetValue(value); }
        public static int MaxElfINT { get => CIMaxElfINT.GetValue(); set => CIMaxElfINT.SetValue(value); }
        public static int MaxElfWIL { get => CIMaxElfWIL.GetValue(); set => CIMaxElfWIL.SetValue(value); }

        // Max Human
        public static int MaxHumanBOD { get => CIMaxHumanBOD.GetValue(); set => CIMaxHumanBOD.SetValue(value); }
        public static int MaxHumanQUI { get => CIMaxHumanQUI.GetValue(); set => CIMaxHumanQUI.SetValue(value); }
        public static int MaxHumanSTR { get => CIMaxHumanSTR.GetValue(); set => CIMaxHumanSTR.SetValue(value); }
        public static int MaxHumanCHA { get => CIMaxHumanCHA.GetValue(); set => CIMaxHumanCHA.SetValue(value); }
        public static int MaxHumanINT { get => CIMaxHumanINT.GetValue(); set => CIMaxHumanINT.SetValue(value); }
        public static int MaxHumanWIL { get => CIMaxHumanWIL.GetValue(); set => CIMaxHumanWIL.SetValue(value); }

        // Max Ork
        public static int MaxOrkBOD { get => CIMaxOrkBOD.GetValue(); set => CIMaxOrkBOD.SetValue(value); }
        public static int MaxOrkQUI { get => CIMaxOrkQUI.GetValue(); set => CIMaxOrkQUI.SetValue(value); }
        public static int MaxOrkSTR { get => CIMaxOrkSTR.GetValue(); set => CIMaxOrkSTR.SetValue(value); }
        public static int MaxOrkCHA { get => CIMaxOrkCHA.GetValue(); set => CIMaxOrkCHA.SetValue(value); }
        public static int MaxOrkINT { get => CIMaxOrkINT.GetValue(); set => CIMaxOrkINT.SetValue(value); }
        public static int MaxOrkWIL { get => CIMaxOrkWIL.GetValue(); set => CIMaxOrkWIL.SetValue(value); }

        // Max Troll
        public static int MaxTrollBOD { get => CIMaxTrollBOD.GetValue(); set => CIMaxTrollBOD.SetValue(value); }
        public static int MaxTrollQUI { get => CIMaxTrollQUI.GetValue(); set => CIMaxTrollQUI.SetValue(value); }
        public static int MaxTrollSTR { get => CIMaxTrollSTR.GetValue(); set => CIMaxTrollSTR.SetValue(value); }
        public static int MaxTrollCHA { get => CIMaxTrollCHA.GetValue(); set => CIMaxTrollCHA.SetValue(value); }
        public static int MaxTrollINT { get => CIMaxTrollINT.GetValue(); set => CIMaxTrollINT.SetValue(value); }
        public static int MaxTrollWIL { get => CIMaxTrollWIL.GetValue(); set => CIMaxTrollWIL.SetValue(value); }

#if NARROWKARMABUTTONS
        public static float? ORIGINAL_BLOCK_WIDTH = null;
        public static float? ORIGINAL_BLOCK_HALF_WIDTH = null;

        public static float NARROW_BLOCK_WIDTH = 40f;

        // assume the worst
        private static int MAX_ALL_ATTRIBUTES
        {
            get
            {
                return 20;
            }
        }

        public static float SCALE_MULTIPLIER
        {
            get
            {
                if (!MaxAttributesOverride || !NarrowKarmaButtons) return 1f;

                return NARROW_BLOCK_WIDTH / (ORIGINAL_BLOCK_WIDTH ?? KarmaBlock.BLOCK_WIDTH);
            }
        }
#endif

        private static void VerifyAttribute(string metahumanType, isogame.Attribute attribute, int configValue, Action<int> OverrideTargetMax)
        {
            if (ResetAllAttributeMaxesToValue > 0)
            {
                OverrideTargetMax(ResetAllAttributeMaxesToValue);
                return;
            }

            int finalResult = configValue;
            if (finalResult <= 0)
            {
                // just normalize to 0 and kick out
                OverrideTargetMax(0);
                return;
            }

            if (finalResult > 20)
            {
                SRPlugin.Logger.LogInfo($"Requested {metahumanType} max {Enum.GetName(typeof(isogame.Attribute), attribute)} value of {finalResult}. Reducing to 20.");
                finalResult = 20;
            }
            if (finalResult != configValue)
            {
                OverrideTargetMax(finalResult);
            }
        }

        public override Version AfterBindMigration(Version startingVersion)
        {
            // need to take care of all of the attributes that moved from the old "Attributes" section to
            // the new feature name based section
            if (startingVersion == null) return null;

            Version targetVersion = new Version(10, 0);
            if (startingVersion < targetVersion)
            {
                string oldAttributesSection = "Attributes";
                string newAttributesSection = SettingsSectionName();

                foreach (ConfigItemBase configItem in configItems)
                {
                    MigrateSetting(oldAttributesSection, configItem);
                }

                return null;
            }

            return null;
        }

        public override void PostApplyPatches()
        {
            // always be cautious
            if (!MaxAttributesOverride) return;

            // verify validity of attribute configuration
            if (ResetAllAttributeMaxesToValue < 0 || ResetAllAttributeMaxesToValue > 20) ResetAllAttributeMaxesToValue = 0;
            if (ResetAllAttributeMaxesToValue > 0)
            {
                SRPlugin.Logger.LogInfo($"This launch only, resetting all configuration max attributes to {ResetAllAttributeMaxesToValue}");
            }

            var
            attr = isogame.Attribute.Attribute_Body; VerifyAttribute("Dwarf", attr, MaxDwarfBOD, v => MaxDwarfBOD = v);
            attr = isogame.Attribute.Attribute_Quickness; VerifyAttribute("Dwarf", attr, MaxDwarfQUI, v => MaxDwarfQUI = v);
            attr = isogame.Attribute.Attribute_Strength; VerifyAttribute("Dwarf", attr, MaxDwarfSTR, v => MaxDwarfSTR = v);
            attr = isogame.Attribute.Attribute_Intelligence; VerifyAttribute("Dwarf", attr, MaxDwarfINT, v => MaxDwarfINT = v);
            attr = isogame.Attribute.Attribute_Willpower; VerifyAttribute("Dwarf", attr, MaxDwarfWIL, v => MaxDwarfWIL = v);
            attr = isogame.Attribute.Attribute_Charisma; VerifyAttribute("Dwarf", attr, MaxDwarfCHA, v => MaxDwarfCHA = v);

            attr = isogame.Attribute.Attribute_Body; VerifyAttribute("Elf", attr, MaxElfBOD, v => MaxElfBOD = v);
            attr = isogame.Attribute.Attribute_Quickness; VerifyAttribute("Elf", attr, MaxElfQUI, v => MaxElfQUI = v);
            attr = isogame.Attribute.Attribute_Strength; VerifyAttribute("Elf", attr, MaxElfSTR, v => MaxElfSTR = v);
            attr = isogame.Attribute.Attribute_Intelligence; VerifyAttribute("Elf", attr, MaxElfINT, v => MaxElfINT = v);
            attr = isogame.Attribute.Attribute_Willpower; VerifyAttribute("Elf", attr, MaxElfWIL, v => MaxElfWIL = v);
            attr = isogame.Attribute.Attribute_Charisma; VerifyAttribute("Elf", attr, MaxElfCHA, v => MaxElfCHA = v);

            attr = isogame.Attribute.Attribute_Body; VerifyAttribute("Human", attr, MaxHumanBOD, v => MaxHumanBOD = v);
            attr = isogame.Attribute.Attribute_Quickness; VerifyAttribute("Human", attr, MaxHumanQUI, v => MaxHumanQUI = v);
            attr = isogame.Attribute.Attribute_Strength; VerifyAttribute("Human", attr, MaxHumanSTR, v => MaxHumanSTR = v);
            attr = isogame.Attribute.Attribute_Intelligence; VerifyAttribute("Human", attr, MaxHumanINT, v => MaxHumanINT = v);
            attr = isogame.Attribute.Attribute_Willpower; VerifyAttribute("Human", attr, MaxHumanWIL, v => MaxHumanWIL = v);
            attr = isogame.Attribute.Attribute_Charisma; VerifyAttribute("Human", attr, MaxHumanCHA, v => MaxHumanCHA = v);

            attr = isogame.Attribute.Attribute_Body; VerifyAttribute("Ork", attr, MaxOrkBOD, v => MaxOrkBOD = v);
            attr = isogame.Attribute.Attribute_Quickness; VerifyAttribute("Ork", attr, MaxOrkQUI, v => MaxOrkQUI = v);
            attr = isogame.Attribute.Attribute_Strength; VerifyAttribute("Ork", attr, MaxOrkSTR, v => MaxOrkSTR = v);
            attr = isogame.Attribute.Attribute_Intelligence; VerifyAttribute("Ork", attr, MaxOrkINT, v => MaxOrkINT = v);
            attr = isogame.Attribute.Attribute_Willpower; VerifyAttribute("Ork", attr, MaxOrkWIL, v => MaxOrkWIL = v);
            attr = isogame.Attribute.Attribute_Charisma; VerifyAttribute("Ork", attr, MaxOrkCHA, v => MaxOrkCHA = v);

            attr = isogame.Attribute.Attribute_Body; VerifyAttribute("Troll", attr, MaxTrollBOD, v => MaxTrollBOD = v);
            attr = isogame.Attribute.Attribute_Quickness; VerifyAttribute("Troll", attr, MaxTrollQUI, v => MaxTrollQUI = v);
            attr = isogame.Attribute.Attribute_Strength; VerifyAttribute("Troll", attr, MaxTrollSTR, v => MaxTrollSTR = v);
            attr = isogame.Attribute.Attribute_Intelligence; VerifyAttribute("Troll", attr, MaxTrollINT, v => MaxTrollINT = v);
            attr = isogame.Attribute.Attribute_Willpower; VerifyAttribute("Troll", attr, MaxTrollWIL, v => MaxTrollWIL = v);
            attr = isogame.Attribute.Attribute_Charisma; VerifyAttribute("Troll", attr, MaxTrollCHA, v => MaxTrollCHA = v);

            if (ResetAllAttributeMaxesToValue > 0)
            {
                ResetAllAttributeMaxesToValue = 0;
            }
        }

        /*
         * This patch is provided as a proof of concept so you can easily see that the plugin, once built and installed, works.
         */
        [HarmonyPatch(typeof(StatsUtil))]
        internal class StatsUtilPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.IsNewEtiquetteLevel))]
            public static void IsNewEtiquetteLevelPostfix(ref bool __result, int charisma)
            {
                if (!MaxAttributesOverride) return;

                if (charisma > (2 * Constants.NUM_ETIQUETTES))
                {
                    __result = false;
                }
            }

            private static void SelectiveApply(int newResult, ref int __result)
            {
                if (newResult > 0)
                {
                    __result = newResult;
                }
            }

            private static int GetMaxMetahumanAttribute(isogame.Attribute attr)
            {
                return Math.Max(StatsUtil.GetAttributeMax_Dwarf(attr),
                    Math.Max(StatsUtil.GetAttributeMax_Elf(attr),
                        Math.Max(StatsUtil.GetAttributeMax_Human(attr),
                            Math.Max(StatsUtil.GetAttributeMax_Ork(attr),
                                StatsUtil.GetAttributeMax_Troll(attr)))));
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax))]
            public static void GetAttributeMax_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributesOverride) return;

                switch (entry)
                {
                    case isogame.Attribute.Attribute_Body:
                        __result = GetMaxMetahumanAttribute(isogame.Attribute.Attribute_Body); break;
                    case isogame.Attribute.Attribute_Quickness:
                        __result = GetMaxMetahumanAttribute(isogame.Attribute.Attribute_Quickness); break;
                    case isogame.Attribute.Attribute_Strength:
                        __result = GetMaxMetahumanAttribute(isogame.Attribute.Attribute_Strength); break;
                    case isogame.Attribute.Attribute_Charisma:
                        __result = GetMaxMetahumanAttribute(isogame.Attribute.Attribute_Charisma); break;
                    case isogame.Attribute.Attribute_Intelligence:
                        __result = GetMaxMetahumanAttribute(isogame.Attribute.Attribute_Intelligence); break;
                    case isogame.Attribute.Attribute_Willpower:
                        __result = GetMaxMetahumanAttribute(isogame.Attribute.Attribute_Willpower); break;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax_Dwarf))]
            public static void GetAttributeMax_Dwarf_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributesOverride) return;

                switch (entry)
                {
                    case isogame.Attribute.Attribute_Body:
                        SelectiveApply(MaxDwarfBOD, ref __result); break;
                    case isogame.Attribute.Attribute_Quickness:
                        SelectiveApply(MaxDwarfQUI, ref __result); break;
                    case isogame.Attribute.Attribute_Strength:
                        SelectiveApply(MaxDwarfSTR, ref __result); break;
                    case isogame.Attribute.Attribute_Charisma:
                        SelectiveApply(MaxDwarfCHA, ref __result); break;
                    case isogame.Attribute.Attribute_Intelligence:
                        SelectiveApply(MaxDwarfINT, ref __result); break;
                    case isogame.Attribute.Attribute_Willpower:
                        SelectiveApply(MaxDwarfWIL, ref __result); break;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax_Elf))]
            public static void GetAttributeMax_Elf_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributesOverride) return;

                switch (entry)
                {
                    case isogame.Attribute.Attribute_Body:
                        SelectiveApply(MaxElfBOD, ref __result); break;
                    case isogame.Attribute.Attribute_Quickness:
                        SelectiveApply(MaxElfQUI, ref __result); break;
                    case isogame.Attribute.Attribute_Strength:
                        SelectiveApply(MaxElfSTR, ref __result); break;
                    case isogame.Attribute.Attribute_Charisma:
                        SelectiveApply(MaxElfCHA, ref __result); break;
                    case isogame.Attribute.Attribute_Intelligence:
                        SelectiveApply(MaxElfINT, ref __result); break;
                    case isogame.Attribute.Attribute_Willpower:
                        SelectiveApply(MaxElfWIL, ref __result); break;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax_Human))]
            public static void GetAttributeMax_Human_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributesOverride) return;

                switch (entry)
                {
                    case isogame.Attribute.Attribute_Body:
                        SelectiveApply(MaxHumanBOD, ref __result); break;
                    case isogame.Attribute.Attribute_Quickness:
                        SelectiveApply(MaxHumanQUI, ref __result); break;
                    case isogame.Attribute.Attribute_Strength:
                        SelectiveApply(MaxHumanSTR, ref __result); break;
                    case isogame.Attribute.Attribute_Charisma:
                        SelectiveApply(MaxHumanCHA, ref __result); break;
                    case isogame.Attribute.Attribute_Intelligence:
                        SelectiveApply(MaxHumanINT, ref __result); break;
                    case isogame.Attribute.Attribute_Willpower:
                        SelectiveApply(MaxHumanWIL, ref __result); break;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax_Ork))]
            public static void GetAttributeMax_Ork_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributesOverride) return;

                switch (entry)
                {
                    case isogame.Attribute.Attribute_Body:
                        SelectiveApply(MaxOrkBOD, ref __result); break;
                    case isogame.Attribute.Attribute_Quickness:
                        SelectiveApply(MaxOrkQUI, ref __result); break;
                    case isogame.Attribute.Attribute_Strength:
                        SelectiveApply(MaxOrkSTR, ref __result); break;
                    case isogame.Attribute.Attribute_Charisma:
                        SelectiveApply(MaxOrkCHA, ref __result); break;
                    case isogame.Attribute.Attribute_Intelligence:
                        SelectiveApply(MaxOrkINT, ref __result); break;
                    case isogame.Attribute.Attribute_Willpower:
                        SelectiveApply(MaxOrkWIL, ref __result); break;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetAttributeMax_Troll))]
            public static void GetAttributeMax_Troll_Postfix(ref int __result, isogame.Attribute entry)
            {
                if (!MaxAttributesOverride) return;

                switch (entry)
                {
                    case isogame.Attribute.Attribute_Body:
                        SelectiveApply(MaxTrollBOD, ref __result); break;
                    case isogame.Attribute.Attribute_Quickness:
                        SelectiveApply(MaxTrollQUI, ref __result); break;
                    case isogame.Attribute.Attribute_Strength:
                        SelectiveApply(MaxTrollSTR, ref __result); break;
                    case isogame.Attribute.Attribute_Charisma:
                        SelectiveApply(MaxTrollCHA, ref __result); break;
                    case isogame.Attribute.Attribute_Intelligence:
                        SelectiveApply(MaxTrollINT, ref __result); break;
                    case isogame.Attribute.Attribute_Willpower:
                        SelectiveApply(MaxTrollWIL, ref __result); break;
                }
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
                if (!MaxAttributesOverride) return true;

                if (!SimulatedClickLastPossible) return true;

                BetterList<KarmaBlock> blockList = AccessTools.Field(typeof(KarmaEntry2), "blockList").GetValue(__instance) as BetterList<KarmaBlock>;
                if (blockList == null)
                {
                    // not sure what happened but let normal code try to handle it
                    return true;
                }

                KarmaScreen2 parentKarmaScreen = AccessTools.Field(typeof(KarmaEntry2), "parentKarmaScreen").GetValue(__instance) as KarmaScreen2;
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
                if (!MaxAttributesOverride) return;

                if (!SimulatedClickLastPossible) return;

                BetterList<KarmaBlock> blockList = AccessTools.Field(typeof(KarmaEntry2), "blockList").GetValue(__instance) as BetterList<KarmaBlock>;
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
                if (!MaxAttributesOverride || !NarrowKarmaButtons) return;

                __instance.transform.localScale = new Vector3(SCALE_MULTIPLIER, 1f, 1f);
            }
        }
#endif
    }
}
