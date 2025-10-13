using ECS;
using ECSGame;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class TimerSystem : AComponentSystem<EcsNode, TimerComponent>,
IAwake<EcsNode, TimerComponent>
    {
        public void Awake(EcsNode entity, TimerComponent component)
        {
        }

        public static void Update(EcsNode entity, TimerComponent component)
        {
            var nowTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

            if (component.TimeList.Count > 0)
            {
                var time = component.TimeList[0];

                if (time < nowTime)
                {
                    component.TimeList.RemoveAt(0);
                    var timers = component.Timers[time];
                    component.Timers.Remove(time);

                    if (timers is ETTask ettask)
                    {
                        ettask.SetResult();
                    }
                    else
                    {
                        var queue = (Queue<ETTask>)timers;
                        while (queue.Count > 0)
                        {
                            var task = queue.Dequeue();
                            task.SetResult();
                        }
                    }
                }
            }
        }

        public static ETTask WaitAsync(EcsEntity entity, long milliSeconds)
        {
            var ettask = ETTask.Create();
            
            var ecsNode = entity.EcsNode;

            var timerComp = ecsNode.GetComponent<TimerComponent>();
            var time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond + milliSeconds;
            if (!timerComp.Timers.ContainsKey(time))
            {
                timerComp.TimeList.Add(time);
                timerComp.Timers.Add(time, ettask);

                timerComp.TimeList.Sort((a, b) =>
                {
                    return a.CompareTo(b);
                });
            }
            else
            {
                var timer = timerComp.Timers[time];
                if (timer is ETTask task)
                {
                    var queue = new Queue<ETTask>();
                    queue.Enqueue(task);
                    queue.Enqueue(ettask);
                    timerComp.Timers[time] = queue;
                }
                else
                {
                    var queue = (Queue<ETTask>)timer;
                    queue.Enqueue(ettask);
                }
            }

            return ettask;
        }
    } 
}
