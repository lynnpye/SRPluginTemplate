
using BepInEx.Configuration;
using HarmonyLib;
using System;

namespace SRPlugin
{
    public delegate ConfigFile GetConfigFileDelegate();

    public class SRPlugin
    {
        public static string HarmonyID { get; private set; }
        public static Harmony Harmony { get; private set; }
        private static GetConfigFileDelegate GetConfigFile;
        public static ConfigFile ConfigFile
        {
            get
            {
                return GetConfigFile.Invoke();
            }
        }

        public static void Awaken(GetConfigFileDelegate configFileDelegate)
        {
            GetConfigFile = configFileDelegate;

            HarmonyID = $"{Guid.NewGuid().ToString()}";

            FeatureConfig.Bind(ConfigFile);

            Harmony = new Harmony(HarmonyID);

            FeatureManager.EnableAllFeaturedPatches();

            // If you aren't managing your Harmony patching directly
            // If you plan to just enable all of your patches immediately
            // Then all you would call here (instead of my FeatureManager above)
            // would be:
            //      HarmonyInst.PatchAll();
            // or something like:
            //      var assembly = Assembly.GetExecutingAssembly();
            //      HarmonyInst.PatachAll(assembly);
            // either of those will search your .dll for properly annotated classes
            // and apply their patches.
        }
    }
}
