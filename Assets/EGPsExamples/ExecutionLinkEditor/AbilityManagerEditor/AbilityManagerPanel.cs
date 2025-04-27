using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using ET;
using UnityEditor;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class AbilityManagerPanel : MonoBehaviour
    {
        public static AbilityManagerPanel Instance { get; set; }
        public Transform AbilityListContentTrans;
        public Transform AbilityConfigItem;
        public Transform EffectListContentTrans;
        public Transform EffectConfigItem;
        public Button SkillBtn;
        public Button StatusBtn;
        public Button EffectBtn;
        public Button PopupBtn;
        public Button RefreshBtn;


        private void Start()
        {
            Instance = this;
            AbilityConfigItem.transform.SetParent(null);
            EffectConfigItem.transform.SetParent(null);
            RefreshList();
            //PopupBtn.onClick.AddListener(Popup);
            //RefreshBtn.onClick.AddListener(RefreshList);
            SkillBtn.onClick.AddListener(ChangeToSkillList);
            StatusBtn.onClick.AddListener(ChangeToSkillList);
            EffectBtn.onClick.AddListener(ChangeToSkillList);
        }

        public void ChangeToSkillList()
        {

        }

        public void Popup()
        {
            var p = transform.rectTransform().anchoredPosition;
            if (p.x > 1)
            {
                transform.rectTransform().anchoredPosition = new Vector2(0, p.y);
            }
            else
            {
                transform.rectTransform().anchoredPosition = new Vector2(transform.rectTransform().sizeDelta.x, p.y);
            }
        }

        public void RefreshList()
        {
#if UNITY_EDITOR
            AbilityListContentTrans.DestroyChildren();
            var allconfigs = AbilityConfigCategory.Instance.GetAll();
            //var guids = UnityEditor.AssetDatabase.FindAssets("t:ExecutionObject");
            foreach (var item in allconfigs)
            {
                //var path = UnityEditor.AssetDatabase.GUIDToAssetPath(item);
                var btn = Instantiate(AbilityConfigItem.gameObject, AbilityListContentTrans);
                btn.transform.Find("AbilityNameText").GetComponent<Text>().text = $"{item.Value.Id} {item.Value.Name}";
                btn.transform.Find("AbilityDesText").GetComponent<Text>().text = item.Value.Description;
                btn.GetComponent<Button>().onClick.AddListener(() => {
                    //var folder = AbilityManagerObject.Instance.SkillAssetFolder;
                    //var obj = AssetDatabase.LoadAssetAtPath<AbilityConfigObject>(folder + $"/Skill_{item.Key}.asset");
                    //UnityEditor.EditorGUIUtility.PingObject(obj);
                    //Selection.activeObject = obj;

                    //EffectListContentTrans.DestroyChildren();
                    //foreach (var item in obj.Effects)
                    //{
                    //    var effectItem = Instantiate(EffectConfigItem, EffectListContentTrans);
                    //    effectItem.Find("EffectText").GetComponent<Text>().text = item.Label;
                    //}
                });
            }

            var listTrm = AbilityListContentTrans;
            var itemTrm = AbilityConfigItem.transform;
            var trackListSize = listTrm.rectTransform().sizeDelta;
            var space = listTrm.GetComponent<VerticalLayoutGroup>().spacing;
            listTrm.rectTransform().sizeDelta = new Vector2(trackListSize.x, listTrm.childCount * (itemTrm.rectTransform().sizeDelta.y + space));
#endif
        }
    }
}
