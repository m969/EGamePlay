using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ET;
using System;
using System.Reflection;

namespace EGamePlay.Combat
{
    public static class ConfigHelper
    {
        public static T Get<T>(int id) where T : ET.IConfig
        {
            return ConfigManageComponent.Instance.Get<T>(id);
        }
    }

    public class ConfigManageComponent : Component
    {
        public static ConfigManageComponent Instance { get; private set; }
        public Dictionary<Type, object> TypeConfigCategarys { get; set; } = new Dictionary<Type, object>();


        public override void Setup(object initData)
        {
            Instance = this;
            var assembly = Assembly.GetAssembly(typeof(ET.TimerComponent));
            var configsCollector = initData as ReferenceCollector;
            foreach (var item in configsCollector.data)
            {
                var configTypeName = $"ET.{item.gameObject.name}";
                var configType = assembly.GetType(configTypeName);
                var typeName = $"ET.{item.gameObject.name}Category";
                var configCategoryType = assembly.GetType(typeName);
                var configCategory = Activator.CreateInstance(configCategoryType) as ACategory;
                configCategory.ConfigText = (item.gameObject as TextAsset).text;
                configCategory.BeginInit();
                TypeConfigCategarys.Add(configType, configCategory);
            }
        }

        public T Get<T>(int id) where T : ET.IConfig
        {
            var category = TypeConfigCategarys[typeof(T)] as ACategory<T>;
            return category.Get(id);
        }
    }
}