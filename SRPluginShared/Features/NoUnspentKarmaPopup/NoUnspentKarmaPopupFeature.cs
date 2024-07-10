using HarmonyLib;
using System.Collections.Generic;

namespace SRPlugin.Features.NoUnspentKarmaPopup
{
    public class NoUnspentKarmaPopupFeature : FeatureImpl
    {
        private static ConfigItem<bool> CINoUnspentKarmaPopup;

        public NoUnspentKarmaPopupFeature()
            : base(
                nameof(NoUnspentKarmaPopup),
                new List<ConfigItemBase>()
                {
                    (CINoUnspentKarmaPopup = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(NoUnspentKarmaPopup), true, "block the popup asking you are you sure you wish to start this scene without spending unspent karma"))
                }, new List<PatchRecord>()
                {
                    PatchRecord.Prefix(
                        typeof(UnspentKarmaListener).GetMethod(nameof(UnspentKarmaListener.CreateUnspentKarmaPopup)),
                        typeof(UnspentKarmaListenerPatch).GetMethod(nameof(UnspentKarmaListenerPatch.CreateUnspentKarmaPopupPatch))
                        )
                })
        {

        }

        public static bool NoUnspentKarmaPopup { get => CINoUnspentKarmaPopup.GetValue(); set => CINoUnspentKarmaPopup.SetValue(value); }

        [HarmonyPatch(typeof(UnspentKarmaListener))]
        internal class UnspentKarmaListenerPatch
        {
            // Setting priority to VeryLow under the assumption that it means other plugins that
            // patch this method will run before me and possibly block me. That's fine.
            // If you were going to do something cool with this screen, more power to you.
            // I just want to block the default behavior.
            [HarmonyPrefix]
            [HarmonyPriority(Priority.VeryLow)]
            [HarmonyPatch(nameof(UnspentKarmaListener.CreateUnspentKarmaPopup))]
            public static bool CreateUnspentKarmaPopupPatch(Player p)
            {
                // this is about as blunt as it gets
                return !NoUnspentKarmaPopupFeature.NoUnspentKarmaPopup;
            }
        }
    }
}
