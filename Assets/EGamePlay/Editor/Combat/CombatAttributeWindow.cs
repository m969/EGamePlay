namespace EGamePlay.Combat
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Sirenix.OdinInspector.Editor;
	using Sirenix.OdinInspector;
	using Sirenix.Utilities.Editor;
	using Sirenix.Utilities;
    using System;
    using System.Linq;

    public class CombatAttributeWindow : OdinEditorWindow
	{
		//[EnumToggleButtons]
		//public ViewTool SomeField;

		private AttributeConfigObject attributeConfigObject;

		[LabelText("属性配置")]
        [ListDrawerSettings(Expanded = true, DraggableItems = false)]
		public List<AttributeConfig> AttributeConfigs;
		//[Button("+")]
		//private void AddAttributeConfig()
		//{
		//	var arr = System.DateTime.Now.Ticks.ToString().Reverse();
		//	AttributeConfigs.Add(new AttributeConfig() { Guid = string.Concat(arr) });
		//}

		[OnInspectorGUI(AppendMethodName = "DrawStateList"/*, AppendMethodName = "EndDrawStateMatrix"*/)]
		[LabelText("状态配置")]
        [ListDrawerSettings(Expanded = true, DraggableItems = false)]
		public List<StateConfig> StateConfigs;
		//[Button("+")]
		//private void AddStateConfig()
		//{
		//	var arr = System.DateTime.Now.Ticks.ToString().Reverse();
		//	StateConfigs.Add(new StateConfig() { Guid = string.Concat(arr) });
		//}

		//[HideLabel]
		//[ReadOnly]
		//[OnInspectorGUI(PrependMethodName = "DrawStateList"/*, AppendMethodName = "EndDrawStateMatrix"*/)]
		//public string SplitTitle = "";

		private void OnEnable()
        {
			attributeConfigObject = AssetDatabase.LoadAssetAtPath<AttributeConfigObject>("Assets/EGamePlay/Demo/Resources/战斗属性配置.asset");
			if (attributeConfigObject == null)
            {

            }
			AttributeConfigs = attributeConfigObject.AttributeConfigs;
			StateConfigs = attributeConfigObject.StateConfigs;
		}

        public void DrawStateList()
		{
			//EditorGUILayout.BeginHorizontal();
			//{
			//	//EditorGUILayout.BeginHorizontal(GUILayout.Width(20));
			//	{
			//		EditorGUILayout.BeginVertical(GUILayout.Width(20));
			//		EditorGUILayout.LabelField("");
			//		EditorGUILayout.LabelField("魔免");
			//		EditorGUILayout.LabelField("魔免");
			//		EditorGUILayout.LabelField("魔免");
			//		EditorGUILayout.LabelField("魔免");
			//		EditorGUILayout.EndVertical();
			//	}
			//	//EditorGUILayout.EndHorizontal();
			//}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.Width(100));
            foreach (var item in StateConfigs)
            {
				EditorGUILayout.LabelField($"{item.AliasName}", GUILayout.Width(100));
			}
			//for (int j = 0; j < 8; j++)
			//{
			//	EditorGUILayout.LabelField("魔免", GUILayout.Width(60));
			//}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginVertical();
			attributeConfigObject.StateMutexTable.SetLength(StateConfigs.Count);
			for (int i = 0; i < StateConfigs.Count; i++)
			{
				//if (attributeConfigObject.StateMutexTable.Count <= i)
    //            {
				//	attributeConfigObject.StateMutexTable.Add(new List<bool>(StateConfigs.Count));
    //            }
				if (attributeConfigObject.StateMutexTable[i] == null)
                {
					attributeConfigObject.StateMutexTable[i] = new List<bool>();
				}
				attributeConfigObject.StateMutexTable[i].SetLength(StateConfigs.Count);
				var item = StateConfigs[i];
				EditorGUILayout.BeginHorizontal(GUILayout.Height(30));
				EditorGUILayout.LabelField($"{item.AliasName}", GUILayout.Width(100));
				for (int j = 0; j < StateConfigs.Count; j++)
				{
					attributeConfigObject.StateMutexTable[i][j] = EditorGUILayout.Toggle(attributeConfigObject.StateMutexTable[i][j], GUILayout.Width(100));
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			//this.BeginWindows();
		}

		////[HorizontalGroup("Split")]
		//[OnInspectorGUI(PrependMethodName ="DrawStateList", AppendMethodName = "EndDrawStateMatrix")]
		//[HideLabel]
		//[TableMatrix(/*HorizontalTitle = "状态排斥表",*/ DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = true, /*RowHeight = 16,*/ HideColumnIndices =true, HideRowIndices =true)]
		private bool[,] CustomCellDrawing = new bool[, ] {
			{false,false,false,false,false,false,false,false, },
			{false,false,false,false,false,false,false,false, },
			{false,false,false,false,false,false,false,false, },
			{false,false,false,false,false,false,false,false, },
			{false,false,false,false,false,false,false,false, },
			{false,false,false,false,false,false,false,false, },
			{false,false,false,false,false,false,false,false, },
			{false,false,false,false,false,false,false,false, },
		};

		private List<List<bool>> StateMutexTable = new List<List<bool>>();

		public void EndDrawStateMatrix()
		{
			//this.EndWindows();
			EditorGUILayout.EndHorizontal();
		}

		//[ShowInInspector, DoNotDrawAsReference]
		//[TableMatrix(HorizontalTitle = "Transposed Custom Cell Drawing", DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, RowHeight = 16, Transpose = true)]
		//public bool[,] Transposed { get { return CustomCellDrawing; } set { CustomCellDrawing = value; } }

		private static bool DrawColoredEnumElement(Rect rect, bool value)
		{
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
			{
				value = !value;
				GUI.changed = true;
				Event.current.Use();
			}

			//EditorGUI.Toggle(rect.Padding(1), value);
			UnityEditor.EditorGUI.DrawRect(rect.Padding(1), value ? new Color(0.8f, 0.2f, 0.2f) : new Color(0, 0, 0, 0.5f));

			return value;
		}

		//[Button("Set")]
		public void TransposeTableMatrixExample()
		{
			// =)
			//this.CustomCellDrawing = new bool[15, 15];
			//this.CustomCellDrawing[6, 5] = true;
			//this.CustomCellDrawing[6, 6] = true;
			//this.CustomCellDrawing[6, 7] = true;
			//this.CustomCellDrawing[8, 5] = true;
			//this.CustomCellDrawing[8, 6] = true;
			//this.CustomCellDrawing[8, 7] = true;
			//this.CustomCellDrawing[5, 9] = true;
			//this.CustomCellDrawing[5, 10] = true;
			//this.CustomCellDrawing[9, 9] = true;
			//this.CustomCellDrawing[9, 10] = true;
			//this.CustomCellDrawing[6, 11] = true;
			//this.CustomCellDrawing[7, 11] = true;
			//this.CustomCellDrawing[8, 11] = true;
		}


		[MenuItem("Tools/EGamePlay/战斗属性编辑界面")]
		private static void ShowWindow()
		{
			var window = GetWindowWithRect<CombatAttributeWindow>(new Rect(0, 0, 800, 600), true, "战斗属性编辑界面");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
		}

		protected override void OnGUI()
		{
			base.OnGUI();
			//DrawStateList();
		}
	}
}