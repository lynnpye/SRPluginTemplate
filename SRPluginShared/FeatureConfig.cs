using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace SRPlugin
{
    internal class FeatureConfig
    {
        private static bool DefaultCheatier = true;
        private static bool DefaultEnableOverrideStartingKarma = true;
        private static int DefaultOverrideStartingKarma = 60;
        private static bool DefaultAlwaysBeSprinting = true;
        private static bool DefaultMaxAttributes20 = true;
#if NARROWKARMABUTTONS
        private static bool DefaultNarrowKarmaButtons = true;
        private static bool DefaultSimulatedClickLastPossible = true;
#endif
        private static bool DefaultNoCostCyberware = true;

        private static bool? NowCheatier = null;
        private static bool? NowEnableOverrideStartingKarma = null;
        private static int? NowOverrideStartingKarma = null;
        private static bool? NowAlwaysBeSprinting = null;
        private static bool? NowMaxAttributes20 = null;
#if NARROWKARMABUTTONS
        private static bool? NowNarrowKarmaButtons = null;
        private static bool? NowSimulatedClickLastPossible = null;
#endif
        private static bool? NowNoCostCyberware = null;

        private static ConfigEntry<bool> ConfigCheatier {  get; set; }
        private static ConfigEntry<bool> ConfigEnableOverrideStartingKarma { get; set; }
        private static ConfigEntry<int> ConfigOverrideStartingKarma { get; set; }
        private static ConfigEntry<bool> ConfigAlwaysBeSprinting { get; set; }
        private static ConfigEntry<bool> ConfigMaxAttributes20 { get; set; }
#if NARROWKARMABUTTONS
        private static ConfigEntry<bool> ConfigNarrowKarmaButtons { get; set; }
        private static ConfigEntry<bool> ConfigSimulatedClickLastPossible { get; set; }
#endif
        private static ConfigEntry<bool> ConfigNoCostCyberware { get; set; }

        private static T GetCEfg<T>(ConfigEntry<T> configEntry, T defaultValue)
        {
            if (configEntry == null)
            {
                return defaultValue;
            }
            return configEntry.Value;
        }

        private static T SetCEfg<T>(ConfigEntry<T> configEntry, T value)
        {
            if (configEntry != null)
            {
                configEntry.Value = value;
            }
            return value;
        }

        public static bool Cheatier
        {
            get
            {
                return GetCEfg(ConfigCheatier, NowCheatier ?? DefaultCheatier);
            }

            set
            {
                NowCheatier = SetCEfg(ConfigCheatier, value);
            }
        }

        public static int OverrideStartingKarma
        {
            get
            {
                return GetCEfg(ConfigOverrideStartingKarma, NowOverrideStartingKarma ?? DefaultOverrideStartingKarma);
            }

            set
            {
                NowOverrideStartingKarma = SetCEfg(ConfigOverrideStartingKarma, value);
            }
        }

        public static bool EnableOverrideStartingKarma
        { 
            get 
            { 
                return GetCEfg(ConfigEnableOverrideStartingKarma, NowEnableOverrideStartingKarma ?? DefaultEnableOverrideStartingKarma);
            }

            set
            {
                NowEnableOverrideStartingKarma = SetCEfg(ConfigEnableOverrideStartingKarma, value);
            }
        }

        public static bool AlwaysBeSprinting
        {
            get
            {
                return GetCEfg(ConfigAlwaysBeSprinting, NowAlwaysBeSprinting ?? DefaultAlwaysBeSprinting);
            }

            set
            {
                NowAlwaysBeSprinting = SetCEfg(ConfigAlwaysBeSprinting, value);
            }
        }

        public static bool MaxAttributes20
        {
            get
            {
                return GetCEfg(ConfigMaxAttributes20, NowMaxAttributes20 ?? DefaultMaxAttributes20);
            }

            set
            {
                NowMaxAttributes20 = SetCEfg(ConfigMaxAttributes20, value);
            }
        }

#if NARROWKARMABUTTONS
        public static bool NarrowKarmaButtons
        {
            get
            {
                return GetCEfg(ConfigNarrowKarmaButtons, NowNarrowKarmaButtons ?? DefaultNarrowKarmaButtons);
            }

            set
            {
                NowNarrowKarmaButtons = SetCEfg(ConfigNarrowKarmaButtons, value);
            }
        }

        public static bool SimulatedClickLastPossible
        {
            get
            {
                return GetCEfg(ConfigSimulatedClickLastPossible, NowSimulatedClickLastPossible ?? DefaultSimulatedClickLastPossible);
            }

            set
            {
                NowSimulatedClickLastPossible = SetCEfg(ConfigSimulatedClickLastPossible, value);
            }
        }
#endif

        public static bool NoCostCyberware
        {
            get
            {
                return GetCEfg(ConfigNoCostCyberware, NowNoCostCyberware ?? DefaultNoCostCyberware);
            }

            set
            {
                NowNoCostCyberware = SetCEfg(ConfigNoCostCyberware, value);
            }
        }

        private const string FEATURES_SECTION = "Features";

        public static FeatureEnum[] EnabledFeatures
        {
            get
            {
                List<FeatureEnum> features = new List<FeatureEnum>();
                var config = SRPlugin.ConfigFile;

                foreach(FeatureEnum fenum in Enum.GetValues(typeof(FeatureEnum)))
                {
                    ConfigDefinition fedef = new ConfigDefinition(FEATURES_SECTION, fenum.ToString());
                    if (!config.ContainsKey(fedef)) continue;

                    ConfigEntryBase baseEntry = config[fedef];
                    if (!typeof(bool).Equals(baseEntry.SettingType)) continue;

                    ConfigEntry<bool> centry = (ConfigEntry<bool>)baseEntry;

                    if (centry.Value) features.Add(fenum);
                }

                return features.ToArray();
            }
        }

        public static void Bind(ConfigFile configFile)
        {
            if (configFile == null)
            {
                return;
            }

            ConfigCheatier = configFile.Bind(FEATURES_SECTION, nameof(FeatureConfig.Cheatier), true, "adds a new, cheatier, cheat bar");

            ConfigEnableOverrideStartingKarma = configFile.Bind(FEATURES_SECTION, nameof(FeatureConfig.EnableOverrideStartingKarma), true, "setting to false prevents patching");
            ConfigOverrideStartingKarma = configFile.Bind(FEATURES_SECTION, nameof(FeatureConfig.OverrideStartingKarma), 60, "-1 also disables even with the feature enabled, but still patched");

            ConfigAlwaysBeSprinting = configFile.Bind(FEATURES_SECTION, nameof(FeatureConfig.AlwaysBeSprinting), true, "makes some of the longer treks not so bad");

            ConfigMaxAttributes20 = configFile.Bind(FEATURES_SECTION, nameof(FeatureConfig.MaxAttributes20), true, "20 fits nicely, you should see my fix for DF lol");
#if NARROWKARMABUTTONS
            ConfigNarrowKarmaButtons = configFile.Bind(FEATURES_SECTION, nameof(FeatureConfig.NarrowKarmaButtons), true, "you'll probably want this, makes the karma buttons narrower to fit all 20 points");
            ConfigSimulatedClickLastPossible = configFile.Bind(FEATURES_SECTION, nameof(FeatureConfig.SimulatedClickLastPossible), true, "In karma spending, clicking on karma block 20 simulates clicking the last open karma block, like in SRHK");
#endif

            ConfigNoCostCyberware = configFile.Bind(FEATURES_SECTION, nameof(FeatureConfig.NoCostCyberware), true, "overrides the call to GetDerivedEssence... free cyberware!");
        }
    }
}
