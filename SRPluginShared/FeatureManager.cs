using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SRPlugin
{
    internal class FeatureManager
    {
        private static Dictionary<FeatureEnum, IFeature> featuresDict = new Dictionary<FeatureEnum, IFeature>();

        public static Dictionary<FeatureEnum, IFeature> Features
        {
            get
            {
                PopulateFeaturesDict();
                return featuresDict;
            }
        }

        private static void PopulateFeaturesDict(bool forceReload = false)
        {
            if (featuresDict == null) featuresDict = new Dictionary<FeatureEnum, IFeature>();

            if (!forceReload && featuresDict.Count > 0) return;

            featuresDict.Clear();

            Assembly assembly = Assembly.GetExecutingAssembly();

            try
            {
                Type fIntf = typeof(IFeature);
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && type.IsClass && type.GetInterfaces().Contains(fIntf))
                    {
                        FeatureClass featureAttr = Attribute.GetCustomAttribute(type, typeof(FeatureClass)) as FeatureClass;
                        if (featureAttr != null)
                        {
                            FeatureEnum classFeature = featureAttr.Feature;
                            IFeature feature = Activator.CreateInstance(type) as IFeature;
                            if (feature != null)
                            {
                                featuresDict[classFeature] = feature;
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Error finding Feature implementations in assembly '{assembly.FullName}': {ex.Message}");
            }
        }

        public static void EnableFeatures(FeatureEnum[] enabledFeatures)
        {
            foreach (FeatureEnum feature in enabledFeatures)
            {
                if (Features.ContainsKey(feature))
                {
                    Features[feature].ApplyPatches();
                }
            }
        }

        public static void EnableAllFeaturedPatches()
        {
            EnableFeatures(FeatureConfig.EnabledFeatures);
        }

    }
}
