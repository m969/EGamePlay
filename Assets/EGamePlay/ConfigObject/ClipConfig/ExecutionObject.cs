using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ET;

#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
using JsonIgnore = MongoDB.Bson.Serialization.Attributes.BsonIgnoreAttribute;
#endif

#if UNITY_EDITOR
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endif

namespace EGamePlay.Combat
{
    [CreateAssetMenu(fileName = "Execution", menuName = "能力/Execution")]
    public class ExecutionObject
#if UNITY
 : ScriptableObject
#endif
    {
        //[DelayedProperty]
        public int AbilityId;

        [ShowInInspector]
        [DelayedProperty]
        [PropertyOrder(-1)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
#if !UNITY
        string name;
#endif

        public double TotalTime;
        [DelayedProperty, JsonIgnore]
        public GameObject ObjAsset;
        public ExecutionTargetInputType TargetInputType;
        [ShowIf("TargetInputType", ExecutionTargetInputType.Point)]
        [LabelText("范围指示器"), JsonIgnore]
        public GameObject RangeIndicatorObjAsset;
        [ShowIf("TargetInputType", ExecutionTargetInputType.Point)]
        [LabelText("目标点指示器"), JsonIgnore]
        public GameObject PointIndicatorObjAsset;
        [ShowIf("TargetInputType", ExecutionTargetInputType.Point)]
        [LabelText("朝向指示器"), JsonIgnore]
        public GameObject DirectionIndicatorObjAsset;
        [LabelText("起始坐标偏移")]
        public Vector3 Offset;

        [ReadOnly, Space(10)]
        public List<ExecuteClipData> ExecuteClips = new List<ExecuteClipData>();

#if UNITY_EDITOR
        [Button("Save Clips")]
        private void SaveClips()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

#if EGAMEPLAY_ET
        [Button("Save Json")]
        private void SaveJson()
        {
            var skillConfigFolder = Application.dataPath + "/../../../SkillConfigs/ExecutionConfigs";
            var filePath = skillConfigFolder + $"/{name}.json";
            Debug.Log(filePath);
            File.WriteAllText(filePath, JsonHelper.ToJson(this));
        }
#endif

        private void BeginBox()
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Save Clips"))
            {
                SaveClips();
            }
#if EGAMEPLAY_ET
            if (GUILayout.Button("Save Json"))
            {
                SaveJson();
            }
#endif
            //GUILayout.Space(10);
            //Sirenix.Utilities.Editor.SirenixEditorGUI.DrawThickHorizontalSeparator();
            //GUILayout.Space(10);
        }

        //[OnInspectorGUI("BeginBox", append: false)]
        //[SerializeField, LabelText("自动重命名")]
        //public bool AutoRename { get { return StatusConfigObject.AutoRenameStatic; } set { StatusConfigObject.AutoRenameStatic = value; } }

        //private void OnEnable()
        //{
        //    StatusConfigObject.AutoRenameStatic = UnityEditor.EditorPrefs.GetBool("AutoRename", true);
        //}

        //private void OnDisable()
        //{
        //    UnityEditor.EditorPrefs.SetBool("AutoRename", StatusConfigObject.AutoRenameStatic);
        //}

        //[OnInspectorGUI]
        //private void OnInspectorGUI()
        //{
        //    if (!AutoRename)
        //    {
        //        return;
        //    }

        //    RenameFile();
        //}

        //[Button("重命名配置文件"), HideIf("AutoRename")]
        private void RenameFile()
        {
            string[] guids = UnityEditor.Selection.assetGUIDs;
            int i = guids.Length;
            if (i == 1)
            {
                string guid = guids[0];
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var so = UnityEditor.AssetDatabase.LoadAssetAtPath<ExecutionObject>(assetPath);
                if (so != this)
                {
                    return;
                }
                var fileName = System.IO.Path.GetFileName(assetPath);
                var newName = $"Execution_{this.AbilityId}";
                if (!fileName.StartsWith(newName))
                {
                    UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
                }
            }
        }
#endif
    }
}
