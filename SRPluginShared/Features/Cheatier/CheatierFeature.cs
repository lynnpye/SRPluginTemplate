using HarmonyLib;
using System;
using UnityEngine;
using SRPlugin;

namespace SRPlugin.Features.Cheatier
{
    [FeatureClass(FeatureEnum.Cheatier)]
    internal class CheatierFeature : IFeature
    {
        public void UnapplyPatches()
        {
            SRPlugin.Harmony.Unpatch(
                typeof(DebugConsole).GetMethod("AfterDebugInput"),
                typeof(DebugConsolePatch).GetMethod(nameof(DebugConsolePatch.AfterDebugInput))
                );

            SRPlugin.Harmony.Unpatch(
                typeof(DebugConsole).GetMethod("DrawCheatBar"),
                typeof(DebugConsolePatch).GetMethod(nameof(DebugConsolePatch.DrawCheatBarPrefix))
                );
        }

        public void ApplyPatches()
        {
            SRPlugin.Harmony.PatchAll(typeof(DebugConsolePatch));
        }

        [HarmonyPatch(typeof(DebugConsole))]
        internal class DebugConsolePatch
        {
            [HarmonyReversePatch]
            [HarmonyPatch(typeof(DebugConsole), "AfterDebugInput")]
            public static void AfterDebugInput(object instance) =>
                // its a stub so it has no initial content
                throw new NotImplementedException("It's a stub");

            [HarmonyPostfix]
            [HarmonyPatch("DrawCheatBar")]
            public static void DrawCheatBarPrefix(DebugConsole __instance)
            {
                if (!FeatureConfig.Cheatier) return;

                try
                {
                    float butHeight = PrivateFieldAccessor.GetPrivateFieldValue<float>(__instance, "butHeight", 28f);
                    bool showCheats = PrivateFieldAccessor.GetPrivateFieldValue<bool>(__instance, "showCheats", false);

                    float num = (float)Screen.width * 0.75f;
                    float left = (float)Screen.width - num - 10f;
                    float top = 200f + 3.6f * butHeight;
                    GUILayout.BeginArea(new Rect(left, top, num, butHeight * 3.2f));
                    GUILayout.BeginVertical(new GUILayoutOption[0]);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    if (showCheats && RunManager.HasInstance())
                    {
                        if (GUILayout.Button("+1APMax", new GUILayoutOption[]
                        {
                        GUILayout.ExpandWidth(false),
                        GUILayout.Height(butHeight)
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
                        GUILayout.Height(butHeight)
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
                        GUILayout.Height(butHeight)
                        }))
                        {
                            Player player = TurnDirector.ActivePlayer;
                            if (player != null)
                            {
                                StatsUtil.SetAttribute(player, global::isogame.Attribute.Attribute_Max_HP, player.baseAttributes.hp + 50);
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
                        GUILayout.Height(butHeight)
                        }))
                        {
                            RunManager.Instance.DirectAddHP(500, null);
                            Logger.Log(LogChannel.CONSOLE_DESIGNER, LogLevel.Info, "HP add 500 (once more, with feeling this time) ");
                            LazySingletonBehavior<Analyzer>.Instance.CountCheat(1);
                            LazySingletonBehavior<InputManager>.Instance.ConsumeFrame();
                        }
                        //
                        if (GUILayout.Button("+1K", new GUILayoutOption[]
                        {
                        GUILayout.ExpandWidth(false),
                        GUILayout.Height(butHeight)
                        }))
                        {
                            RunManager.Instance.DirectAddKarma(1000);
                            Logger.Log(LogChannel.CONSOLE_DESIGNER, LogLevel.Info, "Karma add 1000 ()");
                            LazySingletonBehavior<Analyzer>.Instance.CountCheat(1);
                            LazySingletonBehavior<InputManager>.Instance.ConsumeFrame();
                        }
                        //
                        if (GUILayout.Button("+100k" + Constants.YEN_SIGN, new GUILayoutOption[]
                        {
                        GUILayout.ExpandWidth(false),
                        GUILayout.Height(butHeight)
                        }))
                        {
                            RunManager.Instance.DirectAddNuyen(100000);
                            Logger.Log(LogChannel.CONSOLE_DESIGNER, LogLevel.Info, "Nuyen add 100k (money bags)");
                            LazySingletonBehavior<Analyzer>.Instance.CountCheat(1);
                            LazySingletonBehavior<InputManager>.Instance.ConsumeFrame();
                        }
                    }
                    bool flag = LazySingletonBehavior<Analyzer>.HasInstance() && LazySingletonBehavior<Analyzer>.Instance.CurrentGameCheated();
                    string text4 = ((!showCheats) ? "< Cheatier" : "> Cheatier");
                    if (GUILayout.Button(text4, (!flag) ? __instance.styleTwo : __instance.styleOne,
                        new GUILayoutOption[]
                        {
                        GUILayout.Width(88f),
                        GUILayout.Height(butHeight)
                        }
                       ))
                    {
                        __instance.ToggleCheats();
                        AfterDebugInput(__instance);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }
                catch (Exception e)
                {
                    FileLog.Log(e.Message);
                }
            }
        }
    }
}
