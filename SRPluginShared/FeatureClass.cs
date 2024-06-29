using System;

namespace SRPlugin
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class FeatureClass : Attribute
    {
        public FeatureEnum Feature { get; private set; }

        public FeatureClass(FeatureEnum feature)
        {
            Feature = feature;
        }
    }
}
