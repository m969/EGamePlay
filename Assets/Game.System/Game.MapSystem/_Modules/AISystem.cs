using ECS;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ECSGame
{
    public static class AIActionExtension
    {
        public static AINode NextAction<T>(this AINode aiNode) where T : IAIAction, new()
        {
            var newNode = new AINode()
            {
                Id = aiNode.Id,
                AIBehaviour = aiNode.AIBehaviour,
                Entity = aiNode.Entity,
                AIAction = ReloadSystem.CreateInstance(aiNode.Entity.EcsNode, typeof(T).FullName) as IAIAction,
                PreNode = aiNode,
            };
            return newNode;
        }

        public static AINode StartNode(this AINode aiNode)
        {
            AISystem.StartNode(aiNode);
            return aiNode;
        }

        public static void NodeDepthGC(this AINode aiNode, int depth)
        {
            if (aiNode.PreNode != null)
            {
                aiNode.PreNode.NodeDepthGC(depth + 1);
                if (depth > 5)
                {
                    aiNode.PreNode = null;
                }
            }
        }
    }

    public class AISystem : AComponentSystem<EcsEntity, AIComponent>,
IAwake<EcsEntity, AIComponent>
    {
        public void Awake(EcsEntity entity, AIComponent component)
        {
            var aiNode = new AINode()
            {
                Id = 1,
                AIBehaviour = AIBehaviourType.Patrol,
                Entity = entity,
                AIAction = ReloadSystem.CreateInstance(entity.EcsNode, typeof(MoveInputAIAction).FullName) as IAIAction
            };

            StartNode(aiNode);
        }

        public static void FrameUpdate(EcsEntity entity, AIComponent component, long determineFrame)
        {
            //ConsoleLog.Debug($"AISystem FrameUpdate {component.NodeMap.Count}");
            component.DetermineFrame = determineFrame;
            foreach (var queue in component.NodeMap.Values)
            {
                if (queue.Count == 0)
                {
                    continue;
                }
                var node = queue.Peek();
                //ConsoleLog.Debug($"{node.AIAction.GetType().Name}");
                node.AIAction.Run(node);
            }
        }

        public static void StartNode(AINode aiNode)
        {
            var component = aiNode.Entity.GetComponent<AIComponent>();
            if (!component.NodeMap.ContainsKey(aiNode.Id))
            {
                var queue = new Queue<AINode>();
                queue.Enqueue(aiNode);
                component.NodeMap.Add(aiNode.Id, queue);
            }
            else
            {
                component.NodeMap[aiNode.Id].Enqueue(aiNode);
            }

            aiNode.NodeDepthGC(1);

            aiNode.AIAction.Start(aiNode);
        }

        public static void FinishAndNext(AINode aiNode)
        {
            if (aiNode.AIBehaviour == AIBehaviourType.Patrol)
            {
                PatrolAINext(aiNode);
            }
        }

        public static void PatrolAINext(AINode aiNode)
        {
            var component = aiNode.Entity.GetComponent<AIComponent>();
            var queue = component.NodeMap[aiNode.Id];

            if (aiNode.AIAction is MoveInputAIAction)
            {
                aiNode.NextAction<StopMoveInputAIAction>().StartNode();
            }
            if (aiNode.AIAction is StopMoveInputAIAction)
            {
                aiNode.NextAction<WaitAIAction>().StartNode();
            }
            if (aiNode.AIAction is WaitAIAction)
            {
                aiNode.NextAction<MoveInputAIAction>().StartNode();
            }
            queue.Dequeue();
        }
    }
}
