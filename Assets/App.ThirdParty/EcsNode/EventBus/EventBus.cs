using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public interface IEvent
    {
    }

    // public abstract class AEvent : IEvent
    // {
    // }

    // public abstract class AAsyncEvent : IEvent
    // {
    //     public List<ET.ETTask> HandlerTasks { get; set; } = new List<ET.ETTask>();
    //     public object NetworkMessage { get; set; }
    // }

    public class EventBusNode
    {
        public EventBusNode PreNode { get; set; }
        public EcsNode EcsNode { get; set; }
        public EventBusNode NextNode { get; set; }
    }

    public class EventBus
    {
        public static EventBus Instance = new EventBus();

        //public List<EcsNode> EcsNodes = new List<EcsNode>();
        public EventBusNode FirstNode { get; set; }
        public EventBusNode LastNode { get; set; }


        public void AddEcs(EcsNode ecsNode)
        {
            if (FirstNode == null)
            {
                FirstNode = new EventBusNode() { EcsNode = ecsNode };
                LastNode = FirstNode;
                return;
            }
            LastNode.NextNode = new EventBusNode() { PreNode = LastNode, EcsNode = ecsNode };
            LastNode = LastNode.NextNode;
            //if (!EcsNodes.Contains(ecsNode))
            //{
            //    EcsNodes.Add(ecsNode);
            //}
        }

        public void RemoveEcs(EcsNode ecsNode)
        {
            //if (EcsNodes.Contains(ecsNode))
            //{
            //    EcsNodes.Remove(ecsNode);
            //}
        }

        public void ForeachNodeFromFirst(System.Action<EcsNode> action)
        {
            var node = FirstNode;
            while (node != null)
            {
                action(node.EcsNode);
                node = node.NextNode;
            }
        }

        public void ForeachNodeFromLast(System.Action<EcsNode> action)
        {
            var node = LastNode;
            while (node != null)
            {
                action(node.EcsNode);
                node = node.PreNode;
            }
        }

        public void DriveUpdate()
        {
            var node = FirstNode;
            while (node != null)
            {
                node.EcsNode.DriveEntityUpdate();
                node = node.NextNode;
            }
        }

        public void DriveFixedUpdate()
        {
            var node = FirstNode;
            while (node != null)
            {
                node.EcsNode.DriveEntityFixedUpdate();
                node = node.NextNode;
            }
        }

        public static void Send<T>(T eventObject) where T : IEvent
        {
            Instance.ForeachNodeFromFirst((ecsNode) =>
            {
                ecsNode.Dispatch<IEventHandle<T>>(x => x.OnHandleEvent(ecsNode, eventObject));
            });
        }
    }
}