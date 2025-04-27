using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using ET;
using System;
using System.Reflection;
using System.ComponentModel;
using ECSGame;

namespace EGamePlay
{
    public static class ConfigHelper
    {
        public static T Get<T>(EcsNode entity, int id) where T : class, IConfig
        {
            return ConfigManageSystem.Get<T>(entity, id);
        }

        public static Dictionary<int, T> GetAll<T>(EcsNode entity) where T : class, IConfig
        {
            return ConfigManageSystem.GetAll<T>(entity);
        }
    }

    public class ConfigManageSystem : AComponentSystem<EcsNode, ConfigManageComponent>,
        IAwake<EcsNode, ConfigManageComponent>
    {
        public void Awake(EcsNode entity, ConfigManageComponent component)
        {
            ConfigManageComponent.Instance = component;
            var assembly = Assembly.GetAssembly(typeof(ConfigManageComponent));
            //component.ConfigsCollector = initData as ReferenceCollector;
            if (component.ConfigsCollector == null)
            {
                return;
            }
            foreach (var item in component.ConfigsCollector.data)
            {
                var configTypeName = $"ET.{item.gameObject.name}";
                var configType = assembly.GetType(configTypeName);
                var typeName = $"ET.{item.gameObject.name}Category";
                var configCategoryType = assembly.GetType(typeName);
                var configCategory = Activator.CreateInstance(configCategoryType) as ACategory;
                configCategory.ConfigText = (item.gameObject as TextAsset).text;
                configCategory.BeginInit();
                component.TypeConfigCategarys.Add(configType, configCategory);
            }
        }

        public static T Get<T>(EcsNode entity, int id) where T : class, IConfig
        {
            var category = entity.GetComponent<ConfigManageComponent>().TypeConfigCategarys[typeof(T)] as ACategory<T>;
            return category.Get(id);
        }

        public static Dictionary<int, T> GetAll<T>(EcsNode entity) where T : class, IConfig
        {
            var category = entity.GetComponent<ConfigManageComponent>().TypeConfigCategarys[typeof(T)] as ACategory<T>;
            return category.GetAll();
        }
    }
}