using HarmonyLib;
using Localize;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace SRPlugin.Features.QuitToDesktop
{
    public class QuitToDesktopFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIQuitToDesktop;

        public QuitToDesktopFeature()
            : base(
                  nameof(QuitToDesktop),
                  new List<ConfigItemBase>()
                  {
                      (CIQuitToDesktop = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(QuitToDesktop), true, "adds a button allowing you to quit to desktop from a game")),
                  },
                  new List<PatchRecord>()
                  {
                      PatchRecord.Postfix(
                          AccessTools.Method(typeof(PDAAnchor), "Awake"),
                          typeof(PDAAnchorPatch).GetMethod(nameof(PDAAnchorPatch.AwakePostfix))
                          ),
                  })
        {

        }

        public static bool QuitToDesktop { get => CIQuitToDesktop.GetValue(); set => CIQuitToDesktop.SetValue( value ); }

        [HarmonyPatch(typeof(PDAAnchor))]
        public class PDAAnchorPatch
        {
            private static UISlicedSprite qtdButtonBG;
            private static UILabel qtdButtonText;
            private static UIButton qtdButtonUI;

            private static void CopyTransformValues(Transform target, Transform source)
            {
                target.parent = source.parent;

                target.position = source.position.magnitude * source.position.normalized;
                target.eulerAngles = source.eulerAngles * 1f;

                target.localScale = source.localScale * 1f;
                target.localPosition = source.localPosition.magnitude * source.localPosition.normalized;
                target.localEulerAngles = source.localEulerAngles * 1f;
            }

            [HarmonyPostfix]
            [HarmonyPatch("Awake")]
            public static void AwakePostfix(PDAAnchor __instance)
            {
                if (qtdButtonBG == null)
                {
                    qtdButtonBG = UnityEngine.Object.Instantiate(__instance.restartLevelButtonBG) as UISlicedSprite;
                    qtdButtonText = UnityEngine.Object.Instantiate(__instance.restartLevelButtonText) as UILabel;
                    qtdButtonUI = UnityEngine.Object.Instantiate(__instance.restartLevelButtonUI) as UIButton;

                    CopyTransformValues(qtdButtonBG.transform, __instance.restartLevelButtonBG.transform);
                    CopyTransformValues(qtdButtonText.transform, __instance.restartLevelButtonText.transform);
                    CopyTransformValues(qtdButtonUI.transform, __instance.restartLevelButtonUI.transform);

                    qtdButtonBG.name = "Background";
                    qtdButtonUI.name = "Background";
                    qtdButtonText.name = "Text";

                    // this seems to align everything correctly
                    int moveUp =
#if SRHK
                        150
#else
                        140
#endif
                        ;

                    qtdButtonBG.transform.localPosition = qtdButtonBG.transform.localPosition + Vector3.up * moveUp;
                    qtdButtonText.transform.localPosition = qtdButtonText.transform.localPosition + Vector3.up * moveUp * 0.5f;
                    qtdButtonUI.transform.localPosition = qtdButtonUI.transform.localPosition + Vector3.up * moveUp;

                    // and then we, if we want, we can adjust the whole set per-game
                    var full = new List<Transform>([
                        qtdButtonBG.transform, qtdButtonUI.transform,
                        ]);
                    var mids = new List<Transform>([
                        __instance.restartLevelButtonBG.transform, __instance.restartLevelButtonUI.transform,
                        __instance.loadGameButtonBG.transform, __instance.loadGameButtonUI.transform,
                        __instance.saveGameButtonBG.transform, __instance.saveGameButtonUI.transform,
                        __instance.mainMenuButtonBG.transform, __instance.mainMenuButtonUI.transform,
                        __instance.restartLevelButtonText.transform,
                        __instance.loadGameButtonText.transform,
                        __instance.saveGameButtonText.transform,
                        __instance.mainMenuButtonText.transform,
                        ]);
                    var half = new List<Transform>([
                        qtdButtonText.transform,
                        ]);

                    int moveDown =
#if SRHK
                        25
#else
                        65
#endif
                        ;

                    foreach (Transform t in full)
                    {
                        t.localPosition = t.localPosition + Vector3.down * moveDown;
                    }

                    foreach (Transform t in mids)
                    {
                        t.localPosition = t.localPosition + Vector3.down * moveDown *
#if SRHK
                            0.8f
#else
                            0.65f
#endif
                            ;
                    }

                    foreach (Transform t in half)
                    {
                        t.localPosition = t.localPosition + Vector3.down * moveDown * 0.5f;
                    }

#if SRHK
                    qtdButtonText.text = Strings.T("QUIT TO DESKTOP");
#else
                    qtdButtonText.text = Strings.T("Quit to Desktop");
#endif
                    Utilities.AutoScaleUILabel(ref qtdButtonText);

                    qtdButtonBG.gameObject.AddComponent<UMEventHandler>();

                    BasicButton basicBtn = qtdButtonBG.GetComponent<BasicButton>();
                    if (basicBtn != null)
                    {
                        basicBtn.target = qtdButtonBG.gameObject;
                        basicBtn.buttonName = MSG_QUIT_TO_DESKTOP;
                    }

                    basicBtn = qtdButtonUI.gameObject.GetComponent<BasicButton>();
                    if (basicBtn != null)
                    {
                        basicBtn.target = qtdButtonBG.gameObject;
                        basicBtn.buttonName = MSG_QUIT_TO_DESKTOP;
                    }

                    #region Stuff I added to get the color matching right.
#if SRR || DFDC
                    qtdButtonBG.color = Constants.GREY_COLOR;
                    qtdButtonText.color = Constants.WHITE_COLOR;
#else
                    qtdButtonBG.color = Constants.DARK_GOLD_ALPHA34;
                    qtdButtonText.color = Constants.PALE_GOLD_COLOR;
#endif
                    var boxCollider = qtdButtonBG.GetComponent<BoxCollider>();
                    if (boxCollider != null)
                    {
                        boxCollider.enabled = false;
                    }
#if SRR || DFDC
                    qtdButtonUI.defaultColor = Constants.GREY_COLOR;
#else
                    qtdButtonUI.defaultColor = Constants.DARK_GOLD_ALPHA34;
#endif
                    qtdButtonUI.UpdateColor(true, true);
                    qtdButtonUI.enabled = false;
                    Utilities.AdjustNGUIAlpha(qtdButtonBG.transform, 0.5f, true);
                    Utilities.AdjustNGUIAlpha(qtdButtonText.transform, 0.5f, true);

                    // and now we pretend our flag, or whatever, is true
                        qtdButtonBG.color = Constants.DARK_GOLD_COLOR;
#if SRR || DFDC
                        qtdButtonText.color = Constants.YELLOW_COLOR;
#else
                        qtdButtonText.color = Constants.PALE_GOLD_COLOR;
#endif
                        boxCollider = qtdButtonBG.GetComponent<BoxCollider>();
                        if (boxCollider != null)
                        {
                            boxCollider.enabled = true;
                        }
                        qtdButtonUI.defaultColor = Constants.DARK_GOLD_COLOR;
#if SRHK
                        qtdButtonUI.hover = Constants.ORANGE_COLOR;
                        qtdButtonUI.pressed = Utilities.AdjustHSBColor(Constants.ORANGE_COLOR, 1f, 1f, 0.75f);
#endif
                        qtdButtonUI.UpdateColor(true, true);
                        qtdButtonUI.enabled = true;
                    #endregion

                    string sb = "";

                    sb += "\n\n\n  qtdButtonBG\n===========\n";
                    foreach (Component c in qtdButtonBG.GetComponents<Component>())
                    {
                        sb += $"name:{c.name} type:{c.GetType()}\n";
                    }
                    sb += "==========\n";

                    sb += "\n\n\n  qtdButtonText\n===========\n";
                    foreach (Component c in qtdButtonText.GetComponents<Component>())
                    {
                        sb += $"name:{c.name} type:{c.GetType()}\n";
                    }
                    sb += "==========\n";

                    sb += "\n\n\n  qtdButtonUI\n===========\n";
                    foreach (Component c in qtdButtonUI.GetComponents<Component>())
                    {
                        sb += $"name:{c.name} type:{c.GetType()}\n";
                    }
                    sb += "==========\n";

                    SRPlugin.Squawk(sb);
                }
            }
        }

        private static string MSG_QUIT_TO_DESKTOP = "MSG_QUIT_TO_DESKTOP";

        public class UMEventHandler : MonoBehaviour
        {
            public void OnClickMessage(string message)
            {
                SRPlugin.Squawk($"OnClickMessage:'{message}'");
                if (string.Equals(MSG_QUIT_TO_DESKTOP, message))
                {
                    if (Application.platform == RuntimePlatform.WindowsPlayer)
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                    else
                    {
                        Application.Quit();
                    }
                }
            }
        }
    }
}
