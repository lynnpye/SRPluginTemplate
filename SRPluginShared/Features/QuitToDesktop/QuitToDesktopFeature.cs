using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using Localize;
using UnityEngine;

namespace SRPlugin.Features.QuitToDesktop
{
    public class QuitToDesktopFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIQuitToDesktop;
        private static ConfigItem<bool> CIRequireConfirmation;
        private static ConfigItem<bool> CISkipMainMenuConfirmation;

        public QuitToDesktopFeature()
            : base(
                nameof(QuitToDesktop),
                [
                    (
                        CIQuitToDesktop = new ConfigItem<bool>(
                            PLUGIN_FEATURES_SECTION,
                            nameof(QuitToDesktop),
                            true,
                            "adds a button allowing you to quit to desktop from a game"
                        )
                    ),
                    (
                        CIRequireConfirmation = new ConfigItem<bool>(
                            nameof(RequireConfirmation),
                            true,
                            "require confirmation to quit to desktop, false means one click and you're out so be careful!"
                        )
                    ),
                    (
                        CISkipMainMenuConfirmation = new ConfigItem<bool>(
                            nameof(SkipMainMenuConfirmation),
                            true,
                            "no longer requires confirmation when clicking Exit from the Main Menu"
                        )
                    ),
                ],
                new List<PatchRecord>(
                    PatchRecord.RecordPatches(
                        AccessTools.Method(
                            typeof(PDAAnchorPatch),
                            nameof(PDAAnchorPatch.AwakePostfix)
                        ),
                        AccessTools.Method(typeof(PDAPatch), nameof(PDAPatch.OnEnterMenuPostfix)),
                        AccessTools.Method(
                            typeof(MainMenuScenePatch),
                            nameof(MainMenuScenePatch.OnClickMessagePrefix)
                        )
                    )
                )
                {
                    }
            ) { }

        public static bool QuitToDesktop
        {
            get => CIQuitToDesktop.GetValue();
            set => CIQuitToDesktop.SetValue(value);
        }
        public static bool RequireConfirmation
        {
            get => CIRequireConfirmation.GetValue();
            set => CIRequireConfirmation.SetValue(value);
        }
        public static bool SkipMainMenuConfirmation
        {
            get => CISkipMainMenuConfirmation.GetValue();
            set => CISkipMainMenuConfirmation.SetValue(value);
        }

        // ui element, global for all users
        private static UISlicedSprite qtdButtonBG;
        private static UILabel qtdButtonText;
        private static UIButton qtdButtonUI;
        private static UMEventHandler qtdButtonHandler;

        public static void ExitGameToDesktop()
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

        public static void BypassMainMenuExitConfirmation()
        {
            SRPlugin.Squawk("Skipped confirmation to Exit from MainMenuScene");
            ExitGameToDesktop();
        }

        public static void ConfirmRequest()
        {
            SRPlugin.Squawk("Confirmed request to Quit to Desktop");
            ExitGameToDesktop();
        }

        public static FullscreenPopup qtdConfirmPopup;

        public static void RequestQuitToDesktop()
        {
            if (!RequireConfirmation)
            {
                ConfirmRequest();
                // not strictly needed
                return;
            }

            string title =
#if SRR || DFDC
            Strings.T("Quit to Desktop")
#else
            Strings.T("QUIT TO DESKTOP")
#endif
            ;
            string confirmation = Strings.T(
                "Are you sure you wish to quit to desktop? You will lose any unsaved progress."
            );

            qtdButtonHandler.IsPopupActive = true;
            qtdConfirmPopup = FullscreenPopup.CreateFullscreenPopup(
                title,
                confirmation,
                2,
                0,
                0,
                qtdButtonHandler.gameObject,
                0
            );
        }

        private static void CopyTransformValues(Transform target, Transform source)
        {
            target.parent = source.parent;

            target.position = source.position.magnitude * source.position.normalized;
            target.eulerAngles = source.eulerAngles * 1f;

            target.localScale = source.localScale * 1f;
            target.localPosition = source.localPosition.magnitude * source.localPosition.normalized;
            target.localEulerAngles = source.localEulerAngles * 1f;
        }

        [HarmonyPatch(typeof(MainMenuScene))]
        public class MainMenuScenePatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("OnClickMessage")]
            public static bool OnClickMessagePrefix(string button)
            {
                if (SkipMainMenuConfirmation && string.Equals("exitgame", button))
                {
                    BypassMainMenuExitConfirmation();
                    // not strictly needed
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PDA))]
        public class PDAPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(PDA), "OnEnterMenu")]
            public static void OnEnterMenuPostfix(PDA __instance)
            {
                // actual order
                // start by putting them in a disabled state
#if SRR || DFDC
                qtdButtonBG.color = Constants.GREY_COLOR;
                qtdButtonText.color = Constants.WHITE_COLOR;
                qtdButtonUI.defaultColor = Constants.GREY_COLOR;
#else
                qtdButtonBG.color = Constants.DARK_GOLD_ALPHA34;
                qtdButtonText.color = Constants.PALE_GOLD_COLOR;
                qtdButtonUI.defaultColor = Constants.DARK_GOLD_ALPHA34;
#endif
                qtdButtonUI.UpdateColor(true, true);
                qtdButtonUI.enabled = false;
                Utilities.AdjustNGUIAlpha(qtdButtonBG.transform, 0.5f, true);
                Utilities.AdjustNGUIAlpha(qtdButtonText.transform, 0.5f, true);

                BoxCollider boxCollider = qtdButtonBG.GetComponent<BoxCollider>();
                if (boxCollider != null)
                {
                    boxCollider.enabled = false;
                }

                // and now we act as though our requirements are satisfied to properly respond
                qtdButtonBG.color = Constants.DARK_GOLD_COLOR;
#if SRR || DFDC
                qtdButtonText.color = Constants.YELLOW_COLOR;
#else
                qtdButtonText.color = Constants.PALE_GOLD_COLOR;
#endif
                if (boxCollider != null)
                {
                    boxCollider.enabled = true;
                }
                qtdButtonUI.defaultColor = Constants.DARK_GOLD_COLOR;
#if SRHK
                qtdButtonUI.hover = Constants.ORANGE_COLOR;
                qtdButtonUI.pressed = Utilities.AdjustHSBColor(
                    Constants.ORANGE_COLOR,
                    1f,
                    1f,
                    0.75f
                );
#endif
                qtdButtonUI.UpdateColor(true, true);
                qtdButtonUI.enabled = true;

                Utilities.AdjustNGUIAlpha(qtdButtonBG.transform, 0.5f, true);
                Utilities.AdjustNGUIAlpha(qtdButtonUI.transform, 0.5f, true);

                // do we call RefreshMenu()?
                __instance.RefreshMenu();
            }
        }

        [HarmonyPatch(typeof(PDAAnchor))]
        public class PDAAnchorPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("Awake")]
            public static void AwakePostfix(PDAAnchor __instance)
            {
                // this part of the code is not normally part of Awake as the components would already be created
                // as part of instantiation
                // we create our button based on the Restart level button, and slide it up away from the other buttons
                if (qtdButtonBG == null)
                {
                    qtdButtonBG =
                        UnityEngine.Object.Instantiate(__instance.restartLevelButtonBG)
                        as UISlicedSprite;
                    qtdButtonText =
                        UnityEngine.Object.Instantiate(__instance.restartLevelButtonText)
                        as UILabel;
                    qtdButtonUI =
                        UnityEngine.Object.Instantiate(__instance.restartLevelButtonUI) as UIButton;

                    CopyTransformValues(
                        qtdButtonBG.transform,
                        __instance.restartLevelButtonBG.transform
                    );
                    CopyTransformValues(
                        qtdButtonText.transform,
                        __instance.restartLevelButtonText.transform
                    );
                    CopyTransformValues(
                        qtdButtonUI.transform,
                        __instance.restartLevelButtonUI.transform
                    );

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

                    qtdButtonBG.transform.localPosition =
                        qtdButtonBG.transform.localPosition + (Vector3.up * moveUp);
                    qtdButtonText.transform.localPosition =
                        qtdButtonText.transform.localPosition + (Vector3.up * moveUp * 0.5f);
                    qtdButtonUI.transform.localPosition =
                        qtdButtonUI.transform.localPosition + (Vector3.up * moveUp);

                    // and then we, if we want, we can adjust the whole set per-game
                    List<Transform> full = new([qtdButtonBG.transform, qtdButtonUI.transform,]);
                    List<Transform> mids =
                        new(
                            [
                                __instance.restartLevelButtonBG.transform,
                                __instance.restartLevelButtonUI.transform,
                                __instance.loadGameButtonBG.transform,
                                __instance.loadGameButtonUI.transform,
                                __instance.saveGameButtonBG.transform,
                                __instance.saveGameButtonUI.transform,
                                __instance.mainMenuButtonBG.transform,
                                __instance.mainMenuButtonUI.transform,
                                __instance.restartLevelButtonText.transform,
                                __instance.loadGameButtonText.transform,
                                __instance.saveGameButtonText.transform,
                                __instance.mainMenuButtonText.transform,
                            ]
                        );
                    List<Transform> half = new([qtdButtonText.transform,]);

                    int moveDown =
#if SRHK
                        25
#else
                        65
#endif
                    ;

                    foreach (Transform t in full)
                    {
                        t.localPosition += Vector3.down * moveDown;
                    }

                    foreach (Transform t in mids)
                    {
                        t.localPosition += Vector3.down * moveDown *
#if SRHK
                            0.8f
#else
                            0.65f
#endif
                        ;
                    }

                    foreach (Transform t in half)
                    {
                        t.localPosition += Vector3.down * moveDown * 0.5f;
                    }

                    qtdButtonHandler = qtdButtonBG.gameObject.AddComponent<UMEventHandler>();

                    BasicButton basicBtn = qtdButtonBG.gameObject.GetComponent<BasicButton>();
                    if (basicBtn != null)
                    {
                        basicBtn.target = qtdButtonBG.gameObject;
                        basicBtn.buttonName = UMEventHandler.MSG_QUIT_TO_DESKTOP;
                    }
                    else
                    {
                        SRPlugin.Squawk($"Failed to obtain BasicButton from qtdButtonBG");
                    }

                    basicBtn = qtdButtonUI.gameObject.GetComponent<BasicButton>();
                    if (basicBtn != null)
                    {
                        basicBtn.target = qtdButtonBG.gameObject;
                        basicBtn.buttonName = UMEventHandler.MSG_QUIT_TO_DESKTOP;
                    }
                    else
                    {
                        SRPlugin.Squawk($"Failed to obtain BasicButton from qtdButtonUI");
                    }
                }

                // actual normal steps in Awake

                // set bg/text/ui color values
#if SRR || DFDC
                qtdButtonBG.color = Constants.GREY_COLOR;
                qtdButtonText.color = Constants.WHITE_COLOR;

                qtdButtonUI.duration = 0.1f;
                qtdButtonUI.pressed = Utilities.AdjustHSBColor(
                    Constants.DARK_GOLD_COLOR,
                    1f,
                    1f,
                    0.8f
                );
                qtdButtonUI.hover = Utilities.AdjustHSBColor(
                    Constants.DARK_GOLD_COLOR,
                    1f,
                    1f,
                    0.8f
                );
                qtdButtonUI.disabledColor = Constants.GREY_COLOR;
#else
                qtdButtonBG.color = Constants.DARK_GOLD_ALPHA34;
                qtdButtonText.color = Constants.PALE_GOLD_COLOR;

                qtdButtonUI.duration = 0.1f;
                qtdButtonUI.pressed = Utilities.AdjustHSBColor(
                    Constants.ORANGE_COLOR,
                    1f,
                    1f,
                    0.75f
                );
                qtdButtonUI.hover = Constants.ORANGE_COLOR;
                qtdButtonUI.disabledColor = Constants.DARK_GOLD_ALPHA34;
#endif

                // set text
#if SRHK
                qtdButtonText.text = Strings.T("QUIT TO DESKTOP");
#else
                qtdButtonText.text = Strings.T("Quit to Desktop");
#endif

                // call auto scale on the text
                Utilities.AutoScaleUILabel(ref qtdButtonText);
            }
        }
    }
}
