using UnityEngine.Timeline;
using UnityEngine.Playables;

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

        protected override void OnEnable()
        {
            base.OnEnable();
			attributeConfigObject = AssetDatabase.LoadAssetAtPath<AttributeConfigObject>("Assets/EGamePlay-Examples/RpgExample/Resources/战斗属性配置.asset");
			if (attributeConfigObject == null)
            {
				return;
            }
			AttributeConfigs = attributeConfigObject.AttributeConfigs;
			StateConfigs = attributeConfigObject.StateConfigs;
			Init();
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
			if (attributeConfigObject.StateMutexTable == null)
            {
				attributeConfigObject.StateMutexTable = new List<List<bool>>();
			}
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


        //[MenuItem("Tools/EGamePlay/战斗属性编辑界面")]
        private static void ShowWindow()
		{
			var window = GetWindowWithRect<CombatAttributeWindow>(new Rect(0, 0, 800, 600), true, "战斗属性编辑界面");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
		}

		bool isDraging = false;
		Vector2 pos;
		Texture boxTex;
		Timeline timeline;

		private void Init()
        {
			pos = position.position + new Vector2(200, 200);
			Timeline.segmentTex = boxTex = EditorGUIUtility.Load("Square.png") as Texture;

			timeline = new Timeline() { timelineSegments = new List<TimelineSegment>(), localPosition = pos };
			timeline.AddSegment();
		}

        protected override void OnGUI()
        {
			base.OnGUI();
			//DrawStateList();
			var currentEvent = Event.current;

			if (Timeline.boxStyle == null)
            {
				Timeline.boxStyle = new GUIStyle(GUI.skin.box);
				Timeline.boxStyle.stretchHeight = true;
				Timeline.boxStyle.stretchWidth = true;
			}

			if (currentEvent.isMouse && currentEvent.button == 0)
			{
				foreach (var item in timeline.timelineSegments)
				{
                    switch (currentEvent.type)
                    {
                        case EventType.MouseDown:
							item.DragCheck();
							break;
                        case EventType.MouseUp:
							item.isDraging = false;
							item.isDragingSlider = false;
							break;
                        case EventType.MouseMove:
                            break;
                        case EventType.MouseDrag:
							item.DragCheck();
							break;
                        case EventType.KeyDown:
                            break;
                        case EventType.KeyUp:
                            break;
                        case EventType.ScrollWheel:
                            break;
                        case EventType.Repaint:
                            break;
                        case EventType.Layout:
                            break;
                        case EventType.DragUpdated:
							item.DragCheck();
                            break;
						case EventType.DragPerform:
                            break;
                        case EventType.DragExited:
                            break;
                        case EventType.Ignore:
                            break;
                        case EventType.Used:
                            break;
                        case EventType.ValidateCommand:
                            break;
                        case EventType.ExecuteCommand:
                            break;
                        case EventType.ContextClick:
                            break;
                        case EventType.MouseEnterWindow:
                            break;
                        case EventType.MouseLeaveWindow:
                            break;
                        default:
                            break;
                    }

					if (item.isDraging)
					{
						item.localPosition = new Vector2(currentEvent.delta.x + item.localPosition.x, item.localPosition.y);
					}
					if (item.isDragingSlider)
					{
						item.width = currentEvent.mousePosition.x - item.GetPosition().x;
					}
					//item.DrawSegment();
				}

				//if (currentEvent.type == EventType.MouseDown)
				//{
				//	var clickPos = currentEvent.mousePosition;
				//	if (clickPos.x > pos.x && clickPos.x < (pos.x + 80) && clickPos.y > pos.y && clickPos.y < (pos.y + 20))
				//	{
				//		isDraging = true;
				//	}
				//}
				//if (currentEvent.type == EventType.MouseUp)
				//{
				//	isDraging = false;
				//}
				//if (isDraging)
				//{
				//	pos = currentEvent.delta + pos;
				//}
				//currentEvent.Use();
			}
			timeline.DrawSegments();
			//GUI.Box(new Rect(pos, new Vector2(10, 20)), boxTex);
			//GUI.Box(new Rect(pos+new Vector2(10, 5), new Vector2(80, 10)), boxTex);
			//GUI.Box(new Rect(pos+new Vector2(90, 0), new Vector2(10, 20)), boxTex);
		}
	}

	public class Timeline
    {
		public Vector2 localPosition;
		public List<TimelineSegment> timelineSegments;
		public static Texture segmentTex;
		public static GUIStyle boxStyle;

		public void DrawSegments()
        {
			foreach (var item in timelineSegments)
			{
				item.DrawSegment();
			}
		}

		public void AddSegment()
        {
			timelineSegments.Add(new TimelineSegment() { timeline = this });
		}
    }

	public class TimelineSegment
    {
		public Timeline timeline;
		public Vector2 localPosition;
		public float width = 80;
		public float sliderWidth = 5;
		public bool enter;
		public bool selected;
		public bool isDraging;
		public bool isDragingSlider;

		public Vector2 GetPosition()
        {
			return timeline.localPosition + localPosition;
        }

		public bool IsMouseOnBody()
        {
			var currentEvent = Event.current;
			var position = GetPosition();
			var clickPos = currentEvent.mousePosition;
			if (clickPos.x > (position.x + sliderWidth) && clickPos.x < (position.x + width + sliderWidth) && clickPos.y > position.y && clickPos.y < (position.y + 10))
			{
				return true;
			}
			return false;
        }

		public bool IsMouseOnSlider()
		{
			var currentEvent = Event.current;
			var position = GetPosition();
			var clickPos = currentEvent.mousePosition;
			if (clickPos.x > (position.x + width + sliderWidth) && clickPos.x < (position.x + width + sliderWidth + sliderWidth) && clickPos.y > (position.y - 5) && clickPos.y < (position.y + 15))
			{
				return true;
			}
			return false;
		}

		public void DragCheck()
		{
			if (isDraging || isDragingSlider)
			{
				return;
			}
			if (IsMouseOnBody())
			{
				isDraging = true;
			}
			if (IsMouseOnSlider())
			{
				isDragingSlider = true;
			}
		}

		public void DrawSegment()
        {
			var position = GetPosition();
			//GUI.Box(new Rect(position + new Vector2(sliderWidth, -10), new Vector2(sliderWidth, 20)), Timeline.segmentTex);
			GUI.Box(new Rect(position + new Vector2(sliderWidth, 0), new Vector2(width, 10)), Timeline.segmentTex, Timeline.boxStyle);
			GUI.Box(new Rect(position + new Vector2(width+ sliderWidth, -5), new Vector2(sliderWidth, 20)), Timeline.segmentTex, Timeline.boxStyle);
		}
	}
}