using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;

namespace ECSUnity
{
    [Serializable]
    public class SoundData
    {
        [ToggleGroup("Enable", "$Type")]
        public bool Enable = true;
        [ToggleGroup("Enable", "$Type")]
        public string Type;
        [ToggleGroup("Enable", "$Type")]
        public AudioClip AudioClip;
        [ToggleGroup("Enable", "$Type")]
        [ShowInInspector]
        public float ClipLength
        {
            get
            {
                if (AudioClip == null) return 0;
                return AudioClip.length;
            }
        }
        [ToggleGroup("Enable", "$Type")]
        public float Duration;
        [ToggleGroup("Enable", "$Type")]
        [Range(0f, 1f)]
        public float Volume = 1;
        [ToggleGroup("Enable", "$Type")]
        [Range(-3f, 3f)]
        public float Pitch = 1;
    }

    [CreateAssetMenu(fileName = "MixerConfig", menuName = "MixerConfig")]
    public class MixerConfigObject : ScriptableObject
    {
        public AudioMixerGroup MixerGroup;
        public UnityEngine.Object AudioFolder;
        public List<SoundData> SoundDatas = new();

        [Button("收集音效", ButtonHeight = 25)]
        public void FillNewSounds()
        {
#if UNITY_EDITOR
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(AudioFolder);
            var assetGuids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", new string[] { folderPath });
            foreach ( var assetGuid in assetGuids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(assetGuid);
                var audioClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (SoundDatas.Exists(x => x.AudioClip == audioClip))
                {
                    continue;
                }
                var soundData = new SoundData();
                soundData.AudioClip = audioClip;
                soundData.Duration = audioClip.length;
                SoundDatas.Add(soundData);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
                //Debug.Log($"{audioClip.name}");
            }
#endif
        }
    }
}