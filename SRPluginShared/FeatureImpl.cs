using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace SRPlugin
{
    public abstract class ConfigItemBase
    {
        public abstract void Bind();
        public abstract string GetName();
    }

    public class ConfigItem<T> : ConfigItemBase
    {
        public static ConfigEntry<I_T> Bind<I_T>(string section, string key, I_T defaultValue, string description = null)
        {
            if (description == null)
            {
                return SRPlugin.ConfigFile.Bind(section, key, defaultValue);
            }
            else
            {
                return SRPlugin.ConfigFile.Bind(section, key, defaultValue, description);
            }
        }

        public static I_T GetCEfg<I_T>(ConfigEntry<I_T> configEntry, I_T defaultValue)
        {
            if (configEntry == null)
            {
                return defaultValue;
            }
            return configEntry.Value;
        }

        public static I_T SetCEfg<I_T>(ConfigEntry<I_T> configEntry, I_T value)
        {
            if (configEntry != null)
            {
                configEntry.Value = value;
            }
            return value;
        }

        public string section;
        public string key;
        public string description;

        public ConfigEntry<T> configEntry;
        public T defaultValue;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public T? currentValue;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        public ConfigItem(string section, string key, T defaultValue, string description = null)
        {
            this.section = section;
            this.key = key;
            this.description = description;
            this.defaultValue = defaultValue;
        }

        public Type GetT()
        {
            return typeof(T);
        }

        public void SetValue(T value)
        {
            currentValue = SetCEfg(configEntry, value);
        }

        public T GetValue()
        {
            return GetCEfg(configEntry, currentValue ?? defaultValue);
        }

        public override void Bind()
        {
            configEntry = Bind(section, key, defaultValue, description);
        }

        public override string GetName()
        {
            return key;
        }
    }

    public abstract class FeatureImpl
    {
        public static string FEATURES_SECTION = "Features";

        private List<ConfigItemBase> configItems;

        public FeatureImpl(Func<List<ConfigItemBase>> configItemsFunc)
        {
            this.configItems = configItemsFunc();
        }

        public FeatureImpl(List<ConfigItemBase> configItems)
        {
            this.configItems = configItems;
        }

        public void Bind()
        {
            if (configItems == null) return;
            foreach (var configItem in configItems)
            {
                configItem.Bind();
            }
        }

        public abstract void HandleEnabled();

        public abstract void HandleDisabled();

        public bool IsEnabled()
        {
            if (configItems == null || configItems.Count == 0)
            {
                return false;
            }
            ConfigItem<bool> firstConfigItem = configItems[0] as ConfigItem<bool>;
            return (firstConfigItem != null && firstConfigItem.GetValue());
        }

        public void SetEnabled(bool enabled)
        {
            ConfigItem<bool> firstConfigItem = configItems[0] as ConfigItem<bool>;
            if (firstConfigItem != null)
            {
                firstConfigItem.SetValue(enabled);
            }

            if (IsEnabled())
            {
                HandleEnabled();
            }
            else
            {
                HandleDisabled();
            }
        }
    }
}
