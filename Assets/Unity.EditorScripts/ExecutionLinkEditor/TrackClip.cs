using ET;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EGamePlay
{
    public class TrackClip : MonoBehaviour
    {
        public ExecuteClipType TrackClipType => TrackClipData.ExecuteClipType;
        public RectTransform ClipRect;
        public Color ClipRectColor = Color.white;
        public Color ExecutionClipRectColor = Color.white;
        public Color ParticleEffectRectColor = Color.white;
        [Space(10)]
        public Image ClipTypeBar;
        public Color AnimationClipBarColor = Color.white;
        public Color AudioClipBarColor = Color.white;
        public Color ExecutionClipBarColor = Color.white;
        public Color ParticleEffectBarColor = Color.white;
        [Space(10)]
        public Slider SliderLeft;
        public Slider SliderRight;
        public GameObject ActionImg1;
        public GameObject ActionImg2;
        public RectTransform LeftLine;

        private int panelWidth;
        public ExecuteClipData TrackClipData;
        public Action OnEndDrag;


        // Start is called before the first frame update
        void Start()
        {
            panelWidth = Screen.width - 40;
            //TrackClipData = GetComponent<TrackClipData>();
            //UnityEditor.EditorGUIUtility.AddCursorRect
        }

        // Update is called once per frame
        void Update()
        {
            if (TrackClipData == null)
            {
                return;
            }
            if (TrackClipData.TotalTime <= 0)
            {
                return;
            }

            var timeData = TrackClipData.GetClipTime();
            timeData.StartTime = SliderLeft.value * TrackClipData.TotalTime;
            timeData.EndTime = SliderRight.value * TrackClipData.TotalTime;
            var s = ClipRect.sizeDelta;
            var x = SliderLeft.value * panelWidth;
            var y = SliderRight.value * panelWidth;
            var w = y - x;
            ClipRect.sizeDelta = new Vector2(w, s.y);
            ClipRect.anchoredPosition = new Vector2(SliderLeft.value * panelWidth, 0);
        }

        public void SetClipType(ExecuteClipData clipData)
        {
            //TrackClipData = GetComponent<TrackClipData>();
            TrackClipData = clipData;

            if (TrackClipData.ExecuteClipType == ExecuteClipType.ActionEvent)
            {
                ActionImg1.SetActive(true);
                ActionImg2.SetActive(true);
                var s = ClipRect.sizeDelta;
                ClipRect.sizeDelta = new Vector2(20, s.y);
                SetDragEvent();
                return;
            }

            if (TrackClipData.ExecuteClipType == ExecuteClipType.Animation)
            {
                ClipRect.GetComponent<Image>().color = ClipRectColor;
                ClipTypeBar.color = AnimationClipBarColor;
            }

            if (TrackClipData.ExecuteClipType == ExecuteClipType.ParticleEffect)
            {
                ClipRect.GetComponent<Image>().color = ParticleEffectRectColor;
                ClipTypeBar.color = ParticleEffectBarColor;
            }

            if (TrackClipData.ExecuteClipType == ExecuteClipType.ItemExecute)
            {
                ClipRect.GetComponent<Image>().color = ExecutionClipRectColor;
                ClipTypeBar.color = ExecutionClipBarColor;
                var executeType = TrackClipData.ItemData.MoveType;
                if (executeType == CollisionMoveType.PathFly || executeType == CollisionMoveType.SelectedDirectionPathFly)
                {
                    gameObject.AddComponent<BezierComponent>().CollisionExecuteData = TrackClipData.ItemData;
                }
            }

            SliderRight.value = (float)TrackClipData.GetClipTime().EndTime / (float)TrackClipData.TotalTime;
            SliderLeft.value = (float)TrackClipData.GetClipTime().StartTime / (float)TrackClipData.TotalTime;
            SetDragEvent();
        }

        public void SetDragEvent()
        {
            //TrackClipData = GetComponent<TrackClipData>();
            var trigger = ClipRect.GetComponent<EventTrigger>();

            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(PointerClick);
            trigger.triggers.Add(entry);

            if (TrackClipType != ExecuteClipType.ActionEvent
                && TrackClipType != ExecuteClipType.ParticleEffect
                && TrackClipType != ExecuteClipType.ItemExecute
                && TrackClipType != ExecuteClipType.Animation
                )
            {
                SliderLeft.enabled = false;
                SliderRight.enabled = false;
                if (TrackClipType != ExecuteClipType.Animation)
                {
                    return;
                }
            }

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.BeginDrag;
            entry.callback.AddListener(BeginDrag);
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener(Drag);
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener(EndDrag);
            trigger.triggers.Add(entry);

            SliderLeft.onValueChanged.AddListener(OnSliderLeftChanged);
            SliderRight.onValueChanged.AddListener(OnSliderRightChanged);
        }

        private float oldLeftValue;
        private void OnSliderLeftChanged(float value)
        {
            if ((SliderLeft.value + 0.01f) > SliderRight.value)
            {
                SliderLeft.SetValueWithoutNotify(oldLeftValue);
                return;
            }
            oldLeftValue = value;
        }

        private float oldRightValue;
        private void OnSliderRightChanged(float value)
        {
            if ((SliderLeft.value + 0.01f) > SliderRight.value)
            {
                SliderRight.SetValueWithoutNotify(oldRightValue);
                return;
            }
            oldRightValue = value;
        }

        public void DisableSlider()
        {
            SliderLeft.handleRect.gameObject.SetActive(false);
            SliderRight.handleRect.gameObject.SetActive(false);
        }

        public void EnableSlider()
        {
            SliderLeft.handleRect.gameObject.SetActive(true);
            SliderRight.handleRect.gameObject.SetActive(true);
        }

        private void PointerClick(BaseEventData data)
        {
#if UNITY_EDITOR
            ExecutionLinkPanel.Instance.CurrentExecutionClip = TrackClipData;

            var pEventData = (PointerEventData)data;

            if (pEventData.button == PointerEventData.InputButton.Left)
            {
                ExecutionLinkPanel.Instance.RightContextTrm.gameObject.SetActive(false);
                if (TrackClipType == ExecuteClipType.ItemExecute)
                {
                    var executeType = TrackClipData.ItemData.MoveType;
                    if (executeType == CollisionMoveType.PathFly || executeType == CollisionMoveType.SelectedDirectionPathFly)
                    {
                        UnityEditor.Selection.activeObject = this;
                        UnityEditor.EditorGUIUtility.PingObject(TrackClipData);
                    }
                    else
                    {
                        UnityEditor.Selection.activeObject = TrackClipData;
                        UnityEditor.EditorGUIUtility.PingObject(UnityEditor.Selection.activeObject);
                    }
                }
                else
                {
                    UnityEditor.Selection.activeObject = TrackClipData;
                    UnityEditor.EditorGUIUtility.PingObject(UnityEditor.Selection.activeObject);
                }
            }
            if (pEventData.button == PointerEventData.InputButton.Right)
            {
                ExecutionLinkPanel.Instance.RightContextTrm.gameObject.SetActive(true);
                ExecutionLinkPanel.Instance.RightContextTrm.rectTransform().anchoredPosition = new Vector2(pEventData.position.x, pEventData.position.y - Screen.height);
            }
#endif
        }

        private void BeginDrag(BaseEventData data)
        {
            //DisableSlider();
            LeftLine.gameObject.SetActive(true);
        }

        private void Drag(BaseEventData data)
        {
            var x = (data as PointerEventData).delta.x;
            x = x / panelWidth;
            var minx = x;
            if (x < 0)
            {
                if (SliderLeft.value <= 0)
                {
                    return;
                }
                else
                {
                    minx = Mathf.Max(-SliderLeft.value, x);
                }
            }

            if (TrackClipType == ExecuteClipType.ActionEvent)
            {
                if (x > 0)
                {
                    if (SliderLeft.value >= 1)
                    {
                        return;
                    }
                    else
                    {
                        minx = Mathf.Min(1 - SliderLeft.value, x);
                    }
                }
            }
            else
            {
                if (x > 0)
                {
                    if (SliderRight.value >= 1)
                    {
                        return;
                    }
                    else
                    {
                        minx = Mathf.Min(1 - SliderRight.value, x);
                    }
                }
            }

            //var addtime = minx * TrackClipData.TotalTime;
            //var timeData = TrackClipData.GetTimeClipData();
            //timeData.StartTime += addtime;
            //timeData.StartTime += addtime;

            SliderLeft.SetValueWithoutNotify(SliderLeft .value + minx);
            SliderRight.SetValueWithoutNotify(SliderRight.value + minx);

            //if (TrackClipType == TrackClipType.Action)
            {
                var p = LeftLine.anchoredPosition3D;
                LeftLine.anchoredPosition3D = new Vector2(SliderLeft.value * panelWidth, p.y);
                //Debug.Log($"{SliderLeft.value * panelWidth}");
            }
        }

        private void EndDrag(BaseEventData data)
        {
            //EnableSlider();
            OnEndDrag?.Invoke();
            LeftLine.gameObject.SetActive(false);
        }
    }
}
