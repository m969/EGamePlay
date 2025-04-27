#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

namespace SingularityGroup.HotReload.Demo {
    public class HotReloadBurstJobsDemo : MonoBehaviour {
        public Transform[] cubes;
        public Text informationText;
        public Button openWindowButton;
        public Button openScriptButton;
        public TextAsset thisScript;
        
        TransformAccessArray cubeTransforms;
        CubeJob job;
        void Awake() {
            cubeTransforms = new TransformAccessArray(cubes);
            if(Application.isEditor) {
                openWindowButton.onClick.AddListener(Demo.I.OpenHotReloadWindow);
                openScriptButton.onClick.AddListener(() => Demo.I.OpenScriptFile(thisScript, 49, 17));
            } else {
                openWindowButton.gameObject.SetActive(false);
                openScriptButton.gameObject.SetActive(false);
            }
            informationText.gameObject.SetActive(true);
        }

        void Update() {
            job.deltaTime = Time.deltaTime;
            job.time = Time.time;
            var handle = job.Schedule(cubeTransforms);
            handle.Complete();
            
            if (Demo.I.IsServerRunning()) {
                informationText.text = "Hot Reload is running";
            } else {
                informationText.text = "Hot Reload is not running";
            }
        }
        
        struct CubeJob : IJobParallelForTransform {
            public float deltaTime;
            public float time;
            public void Execute(int index, TransformAccess transform) {
                transform.localRotation *= Quaternion.Euler(50 * deltaTime, 0, 0);
                
                // Uncomment this code to scale the cubes
                // var scale = Mathf.Abs(Mathf.Sin(time));
                // transform.localScale = new Vector3(scale, scale, scale);
                
                // Uncomment this code to make the cube move from left to right and back
                // transform.position += (transform.localScale.x < 0.5 ? Vector3.left : Vector3.right) * deltaTime;
            }
        }
        
        void OnDestroy() {
            cubeTransforms.Dispose();
        }
    }
}
#endif
