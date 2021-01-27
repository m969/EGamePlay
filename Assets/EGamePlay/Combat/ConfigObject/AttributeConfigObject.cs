﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.IO;
using Sirenix.Utilities.Editor;
using System.Linq;

namespace EGamePlay.Combat
{
//#if UNITY_EDITOR
//	[UnityEditor.InitializeOnLoad]
//	public static class UnityInitial
//    {
//		static UnityInitial()
//        {
//			StatusConfigObject.AutoRenameStatic = UnityEditor.EditorPrefs.GetBool("AutoRename", true);
//		}
//	}
//#endif

	[CreateAssetMenu(fileName = "战斗属性配置", menuName = "技能|状态/战斗属性配置")]
    //[LabelText("战斗属性配置")]
    public class AttributeConfigObject : SerializedScriptableObject
    {
		[LabelText("属性配置")]
		public List<AttributeConfig> AttributeConfigs;
		[LabelText("状态配置")]
		public List<StateConfig> StateConfigs;
		[LabelText("状态互斥表")]
		public List<List<bool>> StateMutexTable = new List<List<bool>>();
    }

	[Serializable]
	public class AttributeConfig
	{
		[ToggleGroup("Enable", "@AliasName")]
		public bool Enable;
		[ToggleGroup("Enable")]
		[LabelText("属性名")]
		public string AttributeName = "NewAttribute";
		[ToggleGroup("Enable")]
		[LabelText("属性别名")]
		public string AliasName = "NewAttribute";
		[HideInInspector]
		public string Guid;
	}

	[Serializable]
	public class StateConfig
	{
		[ToggleGroup("Enable", "@AliasName")]
		public bool Enable;
		[ToggleGroup("Enable")]
		[LabelText("状态名")]
		public string StateName = "NewState";
		[ToggleGroup("Enable")]
		[LabelText("状态别名")]
		public string AliasName = "NewState";
		[HideInInspector]
		public string Guid;
	}
}