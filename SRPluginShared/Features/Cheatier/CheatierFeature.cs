using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRPlugin.Features.Cheatier
{
    public class CheatierFeature : FeatureImpl
    {
        private static ConfigItem<bool> CICheatier;

        public CheatierFeature()
            : base(new List<ConfigItemBase>()
            {
                (CICheatier = new ConfigItem<bool>(FEATURES_SECTION, nameof(Cheatier), true, "adds a new, cheatier, cheat bar"))
            }, new List<PatchRecord>()
            {
                PatchRecord.Prefix(
                    //typeof(DebugConsole).GetMethod("DrawCheatBar"),
                    AccessTools.Method(typeof(DebugConsole), "DrawCheatBar"),
                    typeof(DebugConsolePatch).GetMethod(nameof(DebugConsolePatch.DrawCheatBarPrefix))
                    ),
                PatchRecord.Reverse(
                    //typeof(DebugConsole).GetMethod("AfterDebugInput"),
                    AccessTools.Method(typeof(DebugConsole), "AfterDebugInput"),
                    typeof(DebugConsolePatch).GetMethod(nameof(DebugConsolePatch.AfterDebugInputReversePatch))
                    ),
            })
        {
        }

        public static bool Cheatier { get => CICheatier.GetValue(); set => CICheatier.SetValue(value); }

        [HarmonyPatch(typeof(DebugConsole))]
        internal class DebugConsolePatch
        {
            [HarmonyReversePatch]
            [HarmonyPatch(typeof(DebugConsole), "AfterDebugInput")]
            public static void AfterDebugInputReversePatch(object instance) =>
                // its a stub so it has no initial content
                throw new NotImplementedException("It's a stub");

            [HarmonyPostfix]
            [HarmonyPatch("DrawCheatBar")]
            public static void DrawCheatBarPrefix(DebugConsole __instance, float ___butHeight, bool ___showCheats)
            {
                if (!Cheatier) return;

                try
                {
                    float num = (float)Screen.width * 0.75f;
                    float left = (float)Screen.width - num - 10f;
                    float top = 200f + 3.6f * ___butHeight;
                    GUILayout.BeginArea(new Rect(left, top, num, ___butHeight * 3.2f));
                    GUILayout.BeginVertical(new GUILayoutOption[0]);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    if (___showCheats && RunManager.HasInstance())
                    {
                        if (GUILayout.Button("+1APMax", new GUILayoutOption[]
                        {
                        GUILayout.ExpandWidth(false),
                        GUILayout.Height(___butHeight)
                        }))
                        {
                            Player player = TurnDirector.ActivePlayer;
                            if (player != null)
                            {
                                StatsUtil.SetAttribute(player, global::isogame.Attribute.Attribute_AP, player.baseAttributes.ap + 1);
                                RunManager.Instance.DirectAddAP(1, null);
                                Logger.Log(LogChannel.CONSOLE_DESIGNER, LogLevel.Info, "APMax add 1 (you but better) ");
                                LazySingletonBehavior<Analyzer>.Instance.CountCheat(1);
                            }
                            LazySingletonBehavior<InputManager>.Instance.ConsumeFrame();
                        }
                        //
                        if (GUILayout.Button("+100AP", new GUILayoutOption[]
                        {
                        GUILayout.ExpandWidth(false),
                        GUILayout.Height(___butHeight)
                        }))
                        {
                            RunManager.Instance.DirectAddAP(100, null);
                            Logger.Log(LogChannel.CONSOLE_DESIGNER, LogLevel.Info, "AP add 100 (just get it over with already) ");
                            LazySingletonBehavior<Analyzer>.Instance.CountCheat(1);
                            LazySingletonBehavior<InputManager>.Instance.ConsumeFrame();
                        }
                        //
                        if (GUILayout.Button("+50HPMax", new GUILayoutOption[]
                        {
                        GUILayout.ExpandWidth(false),
                        GUILayout.Height(___butHeight)
                        }))
                        {
                            Player player = TurnDirector.ActivePlayer;
                            if (player != null)
                            {
                                player.baseAttributes.hp += 50;
                                RunManager.Instance.DirectAddHP(50, null);
                                Logger.Log(LogChannel.CONSOLE_DESIGNER, LogLevel.Info, "HPMax add 50 (bigger and tougher) ");
                                LazySingletonBehavior<Analyzer>.Instance.CountCheat(1);
                            }
                            LazySingletonBehavior<InputManager>.Instance.ConsumeFrame();
                        }
                        //
                        if (GUILayout.Button("+500HP", new GUILayoutOption[]
                        {
                        GUILayout.ExpandWidth(false),
                        GUILayout.Height(___butHeight)
                        }))
                        {
                            RunManager.Instance.DirectAddHP(500, null);
                            Logger.Log(LogChannel.CONSOLE_DESIGNER, LogLevel.Info, "HP add 500 (once more, with feeling) ");
                            LazySingletonBehavior<Analyzer>.Instance.CountCheat(1);
                            LazySingletonBehavior<InputManager>.Instance.ConsumeFrame();
                        }
                        //
                        if (GUILayout.Button("+1K", new GUILayoutOption[]
                        {
                        GUILayout.ExpandWidth(false),
                        GUILayout.Height(___butHeight)
                        }))
                        {
                            RunManager.Instance.DirectAddKarma(1000);
                            Logger.Log(LogChannel.CONSOLE_DESIGNER, LogLevel.Info, "Karma add 1000 (it was very effective)");
                            LazySingletonBehavior<Analyzer>.Instance.CountCheat(1);
                            LazySingletonBehavior<InputManager>.Instance.ConsumeFrame();
                        }
                        //
                        if (GUILayout.Button("+100k" + Constants.YEN_SIGN, new GUILayoutOption[]
                        {
                        GUILayout.ExpandWidth(false),
                        GUILayout.Height(___butHeight)
                        }))
                        {
                            RunManager.Instance.DirectAddNuyen(100000);
                            Logger.Log(LogChannel.CONSOLE_DESIGNER, LogLevel.Info, "Nuyen add 100k (money bags)");
                            LazySingletonBehavior<Analyzer>.Instance.CountCheat(1);
                            LazySingletonBehavior<InputManager>.Instance.ConsumeFrame();
                        }
                    }
#if !SRR
                    bool flag = LazySingletonBehavior<Analyzer>.HasInstance() && LazySingletonBehavior<Analyzer>.Instance.CurrentGameCheated();
                    string text4 = ((!___showCheats) ? "< Cheatier" : "> Cheatier");
                    if (GUILayout.Button(text4, (!flag) ? __instance.styleTwo : __instance.styleOne,
                        new GUILayoutOption[]
                        {
                            GUILayout.Width(88f),
                            GUILayout.Height(___butHeight)
                        }
                       ))
                    {
                        __instance.ToggleCheats();
                        AfterDebugInputReversePatch(__instance);
                    }
#else
                    string text5 = ((!___showCheats) ? "< Cheatier" : "> Cheatier");
                    if (GUILayout.Button(text5, new GUILayoutOption[]
                    {
                        GUILayout.Width(88f),
                        GUILayout.Height(___butHeight)
                    }))
                    {
                        __instance.ToggleCheats();
                        AfterDebugInputReversePatch(__instance);
                    }
#endif
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }
                catch (Exception e)
                {
                    SRPlugin.Logger.LogInfo($"Exception while drawing Cheatier cheat bar:\n{e}");
                }
            }
        }
    }
}
