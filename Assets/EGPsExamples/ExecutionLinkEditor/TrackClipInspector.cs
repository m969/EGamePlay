#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EGamePlay
{
    [CustomEditor(typeof(TrackClip))]
    public class TrackClipInspector : Editor
    {
        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneViewGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneViewGUI;
        }

        //private void OnSceneGUI()
        //{
        //    OnSceneViewGUI(null);
        //}

        void OnSceneViewGUI(SceneView sceneView)
        {
            //var TrackClipData = (target as TrackClip).TrackClipData;
            //if (TrackClipData == null)
            //{
            //    return;
            //}
            //if (TrackClipData.TrackClipType == TrackClipType.ExecutionClip)
            //{
            //    if (TrackClipData.ExecutionClipData.ExecuteType == ColliderType.PathFly)
            //    {
            //        var be = TrackClipData.ExecutionClipData;
            //        //Log.Debug($"OnSceneViewGUI 3 {TrackClipData.ExecutionClipData.ExecuteType}");
            //        if (be != null && be.Points.Count >= 2)
            //        {
            //            for (int i = 0; i < be.Points.Count; i++)
            //            {
            //                be.Points[i] = Handles.PositionHandle(be.Points[i], Quaternion.identity);
            //                if (i > 0)
            //                {
            //                    Handles.DrawBezier(be.Points[i - 1], be.Points[i], Vector3.up, Vector3.up, Color.black, null, 5f);
            //                }
            //            }
            //            //var startTangent = Handles.PositionHandle(Vector3.zero, Quaternion.identity);
            //            //var endTangent = Handles.PositionHandle(Vector3.zero, Quaternion.identity);
            //        }
            //    }
            //}
        }
    }
}
#endif