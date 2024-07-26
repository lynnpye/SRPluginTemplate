using UnityEngine;

namespace SRPlugin.Features.QuitToDesktop
{
    public class UMEventHandler : MonoBehaviour
    {
        public static string MSG_QUIT_TO_DESKTOP = "MSG_QUIT_TO_DESKTOP";

        public void OnClickMessage(string message)
        {
            if (string.Equals(MSG_QUIT_TO_DESKTOP, message))
            {
                QuitToDesktopFeature.RequestQuitToDesktop();
            }
        }

        public bool IsPopupActive { get; set; }

        public void OnPopupClosed(FullscreenPopup.PopupContents result)
        {
            if (!IsPopupActive)
            {
                return;
            }

            IsPopupActive = false;

            if (result.result == FullscreenPopup.PopupResult.POSITIVE)
            {
                QuitToDesktopFeature.ConfirmRequest();
            }
        }
    }
}
