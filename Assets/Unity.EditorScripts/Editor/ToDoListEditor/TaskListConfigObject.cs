using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.Utilities.Editor;

namespace ToDoListEditor
{
    [Serializable]
    public class TaskConfig
    {
        public const string TitleStr = "待办清单";

        //[ToggleGroup("Enabled", "@Title")]
        //[FoldoutGroup("@Title")]
        //public bool Enabled;
        //[ToggleGroup("Enabled")]
        [FoldoutGroup("@Title")]
        [HideLabel]
        [GUIColor(.8f, .8f, .8f)]
        public string Title = "任务标题";
        //[ToggleGroup("Enabled")]
        [FoldoutGroup("@Title")]
        [HideLabel]
        [TextArea(2, 4)]
        [GUIColor(1,1,1,.8f)]
        public string Description = "描述";
    }

    [CreateAssetMenu(fileName = TaskConfig.TitleStr, menuName = TaskConfig.TitleStr)]
    public class TaskListConfigObject : SerializedScriptableObject
    {
        [LabelText("·")]
        //[HideLabel]
        [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowItemCount = false)]
        public List<TaskConfig> TaskConfigs = new List<TaskConfig>();
    }

//#if UNITY_EDITOR
    [CustomEditor(typeof(TaskListConfigObject))]
    public class TaskListConfigObjectInspector : OdinEditor
    {
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            EditorGUILayout.Space(10);
            //SirenixEditorGUI.Title(TaskConfig.TitleStr, "", TextAlignment.Center, false, boldLabel: true);
        }

        public override void OnInspectorGUI()
        {
            ForceHideMonoScriptInEditor = true;
            base.OnInspectorGUI();
            ForceHideMonoScriptInEditor = false;
            return;
            //var taskListConfigObject = target as TaskListConfigObject;
            //TaskConfig remove = null;
            //foreach (var taskConfig in taskListConfigObject.TaskConfigs)
            //{
            //    EditorGUILayout.BeginHorizontal();

            //    EditorGUILayout.BeginVertical();

            //    var visible = EditorPrefs.GetBool($"{taskConfig.GetHashCode().ToString()}", false);
            //    var toggeled = SirenixEditorGUI.BeginToggleGroup(taskConfig, ref taskConfig.Enabled, ref visible, taskConfig.Title, 0.3f);
            //    if (toggeled)
            //    {
            //        taskConfig.Title = EditorGUILayout.TextField(taskConfig.Title);
            //        taskConfig.Description = EditorGUILayout.TextArea(taskConfig.Description);
            //    }
            //    EditorPrefs.SetBool($"{taskConfig.GetHashCode()}", visible);
            //    SirenixEditorGUI.EndToggleGroup();

            //    EditorGUILayout.EndVertical();

            //    if (GUILayout.Button("-", GUILayout.Width(20)))
            //    {
            //        remove = taskConfig;
            //    }

            //    EditorGUILayout.EndHorizontal();
            //}
            //if (remove != null)
            //{
            //    taskListConfigObject.TaskConfigs.Remove(remove);
            //    EditorUtility.SetDirty(taskListConfigObject);
            //}
            //if (GUILayout.Button("+"))
            //{
            //    taskListConfigObject.TaskConfigs.Add(new TaskConfig());
            //    EditorUtility.SetDirty(taskListConfigObject);
            //}
            //serializedObject.ApplyModifiedProperties();
            //serializedObject.UpdateIfRequiredOrScript();
        }
    }
//#endif
}