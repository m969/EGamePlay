//using Sirenix.OdinInspector.Editor;
//using Sirenix.Utilities;
//using Sirenix.Utilities.Editor;
//using UnityEditor;
//using UnityEngine;
//using System.Linq;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using Object = UnityEngine.Object;
//using Sirenix.OdinInspector.Demos.RPGEditor;

//public class SkillEditorWindow : OdinMenuEditorWindow
//{
//    [MenuItem("Tools/SkillEditorWindow")]
//    private static void Open()
//    {
//        var window = GetWindow<SkillEditorWindow>();
//        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 800);
//    }

//    int totalCount = 0;
//    protected override OdinMenuTree BuildMenuTree()
//    {
//        var tree = new OdinMenuTree(true);
//        tree.DefaultMenuStyle.IconSize = 32.00f;
//        tree.Config.DrawSearchToolbar = true;

//        tree.AddAllAssetsAtPath("", "Assets/Plugins/Sirenix/Demos/SAMPLE - RPG Editor/Items", typeof(Item), true);
//        //.ForEach(this.AddDragHandles);

//        tree.EnumerateTree().AddIcons<Item>(x => x.Icon);

//        return tree;
//    }

//    protected override void OnBeginDrawEditors()
//    {
//        var selected = this.MenuTree.Selection.FirstOrDefault();
//        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

//        bool changed = false;
//        //SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
//        //{
//        //    EditorGUILayout.ObjectField(selected?.Value as GIDTarget, typeof(GIDTarget), false);
//        //    if (GUILayout.Button("Select In Editor"))
//        //    {
//        //        Selection.objects = this.MenuTree.Selection.Where(_ => _.Value is GIDTarget)
//        //            .Select(_ => _.Value as Object).ToArray();
//        //    }
//        //}
//        //SirenixEditorGUI.EndHorizontalToolbar();
//        if (changed)
//        {
//            ForceMenuTreeRebuild();
//        }
//    }
//}