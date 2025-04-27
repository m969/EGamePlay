#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SingularityGroup.HotReload {
    class QuestionDialog : MonoBehaviour {

        [Header("Information")]
        public Text textSummary;
        public Text textSuggestion;

        [Header("UI controls")]
        public Button buttonContinue;
        public Button buttonCancel;
        public Button buttonMoreInfo;
        
        public TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();

        public void UpdateView(Config config) {
            textSummary.text = config.summary;
            textSuggestion.text = config.suggestion;

            if (string.IsNullOrEmpty(config.continueButtonText)) {
                buttonContinue.enabled = false;
            } else {
                buttonContinue.GetComponentInChildren<Text>().text = config.continueButtonText;
                buttonContinue.onClick.AddListener(() => {
                    completion.TrySetResult(true);
                    Hide();
                });
            }

            if (string.IsNullOrEmpty(config.cancelButtonText)) {
                buttonCancel.enabled = false;
            } else {
                buttonCancel.GetComponentInChildren<Text>().text = config.cancelButtonText;
                buttonCancel.onClick.AddListener(() => {
                    completion.TrySetResult(false);
                    Hide();
                });
            }
            
            buttonMoreInfo.onClick.AddListener(() => {
                Application.OpenURL(config.moreInfoUrl);
            });
        }

        internal class Config {
            public string summary;
            public string suggestion;
            public string continueButtonText = "Continue";
            public string cancelButtonText = "Cancel";
            public string moreInfoUrl = "https://hotreload.net/documentation#handling-different-commits";
        }

        /// hide this dialog
        void Hide() {
            gameObject.SetActive(false); // this should disable the Update loop?
        }
    }
}
#endif
