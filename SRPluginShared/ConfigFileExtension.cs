﻿using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;

namespace SRPlugin
{
    public static class ConfigFileExtension
    {
        public static Dictionary<ConfigDefinition, string> GetOrphans(this ConfigFile file)
        {
            var orphanGrabber = AccessTools.PropertyGetter(typeof(ConfigFile), "OrphanedEntries");
            return orphanGrabber.Invoke(SRPlugin.ConfigFile, null)
                as Dictionary<ConfigDefinition, string>;
        }
    }
}
