using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson.Extensions;
//using JsonIgnore = MongoDB.Bson.Serialization.Attributes.BsonIgnoreAttribute;
using System.IO;
using ET;

#if UNITY_EDITOR
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endif

namespace EGamePlay.Combat
{
    [CreateAssetMenu(fileName = "Execution", menuName = "技能|状态/Execution")]
    public class ExecutionObject
#if UNITY
 : ScriptableObject
#endif
    {
        [DelayedProperty]
        public string Id;
        public float TotalTime;
        //public string ObjAssetName;
        //[OnValueChanged("OnValueChanged")]
        [DelayedProperty]
        public GameObject ObjAsset;
        public ExecutionTargetInputType TargetInputType;
        [ShowIf("TargetInputType", ExecutionTargetInputType.Point)]
        [LabelText("范围指示器")]
        public GameObject RangeIndicatorObjAsset;
        [ShowIf("TargetInputType", ExecutionTargetInputType.Point)]
        [LabelText("目标点指示器")]
        public GameObject PointIndicatorObjAsset;
        [ShowIf("TargetInputType", ExecutionTargetInputType.Point)]
        [LabelText("朝向指示器")]
        public GameObject DirectionIndicatorObjAsset;
        //public string BindSkillName;
        //public SkillConfigObject BindSkill;
        //public ExecutionItem ExecutionItem;
        //[PreviouslySerializedAs("ExecutionClips")]
        [ReadOnly, Space(10)]
        public List<ExecuteClipData> ExecuteClips = new List<ExecuteClipData>();

#if UNITY_EDITOR
        //private void OnValueChanged()
        //{
        //    if (ObjAsset != null)
        //    {
        //        ObjAssetName = ObjAsset.name;
        //    }
        //}

        //[Button("Save Clips")]
        private void SaveClips()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            //Log.Debug("SaveObject");
        }

        private void BeginBox()
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Save Clips"))
            {
                SaveClips();
            }
            GUILayout.Space(10);
            Sirenix.Utilities.Editor.SirenixEditorGUI.DrawThickHorizontalSeparator();
            GUILayout.Space(10);
        }

        [OnInspectorGUI("BeginBox", append: false)]
        [SerializeField, LabelText("自动重命名")]
        public bool AutoRename { get { return StatusConfigObject.AutoRenameStatic; } set { StatusConfigObject.AutoRenameStatic = value; } }

        private void OnEnable()
        {
            StatusConfigObject.AutoRenameStatic = UnityEditor.EditorPrefs.GetBool("AutoRename", true);
        }

        private void OnDisable()
        {
            UnityEditor.EditorPrefs.SetBool("AutoRename", StatusConfigObject.AutoRenameStatic);
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (!AutoRename)
            {
                return;
            }

            RenameFile();
        }

        [Button("重命名配置文件"), HideIf("AutoRename")]
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
                var newName = $"Execution_{this.Id}";
                if (!fileName.StartsWith(newName))
                {
                    UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
                }
            }
        }
#endif
    }
}
