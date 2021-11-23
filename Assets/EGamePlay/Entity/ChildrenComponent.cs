using System;
using System.Collections.Generic;


//namespace EGamePlay
//{
//    public partial class Entity/*ChildrenComponent : Component*/
//    {
//        //public List<Entity> Children { get; private set; } = new List<Entity>();
//        //public Dictionary<long, Entity> Id2Children { get; private set; } = new Dictionary<long, Entity>();
//        //public Dictionary<Type, List<Entity>> Type2Children { get; private set; } = new Dictionary<Type, List<Entity>>();


//        //public void SetChild(Entity child)
//        //{
//        //    var childrenComponent = this;
//        //    var Children = childrenComponent.Children;
//        //    var Type2Children = childrenComponent.Type2Children;
//        //    Children.Add(child);
//        //    if (!Type2Children.ContainsKey(child.GetType())) Type2Children.Add(child.GetType(), new List<Entity>());
//        //    Type2Children[child.GetType()].Add(child);
//        //}

//        //public void RemoveChild(Entity child)
//        //{
//        //    var childrenComponent = this;
//        //    var Children = childrenComponent.Children;
//        //    var Type2Children = childrenComponent.Type2Children;
//        //    Children.Remove(child);
//        //    if (Type2Children.ContainsKey(child.GetType())) Type2Children[child.GetType()].Remove(child);
//        //}

//        //public Entity[] GetChildren()
//        //{
//        //    return Children.ToArray();
//        //}

//        //public Entity[] GetTypeChildren<T>() where T : Entity
//        //{
//        //    return Type2Children[typeof(T)].ToArray();
//        //}
//    }
//}