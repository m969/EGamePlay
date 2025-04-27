#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using UnityEngine;
using UnityEngine.UI;

namespace SingularityGroup.HotReload {
    class InstallQRDialog : MonoBehaviour {
        public Button buttonGo;
        public Button buttonHide;

        private void Start() {
            buttonHide.onClick.AddListener(Hide);

            // launch camera app that can scan QR-Code  https://singularitygroup.atlassian.net/browse/SG-29495
            buttonGo.onClick.AddListener(() => {
                Hide();
                var recommendedQrCodeApp = "com.scanteam.qrcodereader";
                Application.OpenURL($"https://play.google.com/store/apps/details?id={recommendedQrCodeApp}");
            });
        }

        /// hide this dialog
        void Hide() {
            gameObject.SetActive(false); // this should disable the Update loop?
        }
    }
}
#endif
