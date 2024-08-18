//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityEngine;
//using EGamePlay;
//using EGamePlay.Combat;
//using ET;

///// <summary>
///// 装备组件
///// </summary>
//public class EquipmentComponent : EGamePlay.Component
//{
//    public Dictionary<long, ItemData> ItemDatas { get; set; } = new Dictionary<long, ItemData>();
//    public Dictionary<long, FloatModifier> EquipmentNumerics { get; set; } = new Dictionary<long, FloatModifier>();


//    public void AddItemData(ItemData itemData)
//    {
//        ItemDatas.Add(itemData.Id, itemData);
//        var equipmentConfig = ConfigHelper.Get<EquipmentConfig>(itemData.ConfigId);
//        var modifier = new FloatModifier();
//        modifier.Value = equipmentConfig.Value;
//        Entity.GetComponent<AttributeComponent>().GetNumeric(equipmentConfig.Attribute).AddModifier(ModifierType.Add, modifier);
//        EquipmentNumerics.Add(itemData.Id, modifier);
//    }

//    public void RemoveItemData(long Id)
//    {
//        var itemData = ItemDatas[Id];
//        var equipmentConfig = ConfigHelper.Get<EquipmentConfig>(itemData.ConfigId);
//        var modifier = EquipmentNumerics[itemData.Id];
//        Entity.GetComponent<AttributeComponent>().GetNumeric(equipmentConfig.Attribute).RemoveModifier(ModifierType.Add, modifier);
//        ItemDatas.Remove(Id);
//    }
//}
