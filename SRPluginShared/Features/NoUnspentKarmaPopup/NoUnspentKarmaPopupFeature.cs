using System.Collections.Generic;
using HarmonyLib;
using isogame;

namespace SRPlugin.Features.NoUnspentKarmaPopup
{
    public class NoUnspentKarmaPopupFeature : FeatureImpl
    {
        private static ConfigItem<bool> CINoUnspentKarmaPopup;

        public NoUnspentKarmaPopupFeature()
            : base(
                nameof(NoUnspentKarmaPopup),
                [
                    (
                        CINoUnspentKarmaPopup = new ConfigItem<bool>(
                            PLUGIN_FEATURES_SECTION,
                            nameof(NoUnspentKarmaPopup),
                            true,
                            "block the popup asking you are you sure you wish to start this scene without spending unspent karma"
                        )
                    )
                ],
                new List<PatchRecord>(
                    PatchRecord.RecordPatches(
                        AccessTools.Method(
                            typeof(SceneDefPatch),
                            nameof(SceneDefPatch.show_equip_screen_on_scene_load_postfix)
                        )
                    )
                )
                {
                    }
            ) { }

        public static bool NoUnspentKarmaPopup
        {
            get => CINoUnspentKarmaPopup.GetValue();
            set => CINoUnspentKarmaPopup.SetValue(value);
        }

        [HarmonyPatch(typeof(SceneDef))]
        internal class SceneDefPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(SceneDef.show_equip_screen_on_scene_load), MethodType.Getter)]
            public static void show_equip_screen_on_scene_load_postfix(ref bool __result)
            {
                SRPlugin.Squawk($"show_equip_screen_on_scene_load_postfix is always false");
                __result = false;
            }
        }
    }
}
