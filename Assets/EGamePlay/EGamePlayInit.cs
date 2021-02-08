using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;


public class EGamePlayInit : SerializedMonoBehaviour
{
    public static EGamePlayInit Instance { get; private set; }
    public Dictionary<string, TextAsset> TypeConfigTexts;
    public Dictionary<string, object> TypeConfigCategarys { get; set; } = new Dictionary<string, object>();


    private void Awake()
    {
        Instance = this;

        ACategory configCategory = new UnitConfigCategory();
        configCategory.BeginInit();
        TypeConfigCategarys.Add("UnitConfig", configCategory);

        configCategory = new SkillConfigCategory();
        configCategory.BeginInit();
        TypeConfigCategarys.Add("SkillConfig", configCategory);

        configCategory = new StatusConfigCategory();
        configCategory.BeginInit();
        TypeConfigCategarys.Add("StatusConfig", configCategory);



        MasterEntity.Create();
        Entity.Create<TimerComponent>();
        Entity.Create<CombatContext>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        MasterEntity.Instance.Update();
        TimerComponent.Instance.Update();
    }

    private void OnApplicationQuit()
    {
        Entity.Destroy(MasterEntity.Instance);
        MasterEntity.Destroy();
    }
}
