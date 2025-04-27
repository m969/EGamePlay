using ECS;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class FrameTimer
    {
        public long EndFrame {  get; set; }
        public long TimerFrame {  get; set; }
        public bool Repeat {  get; set; }
    }

    public enum TimerProgress
    {
        Waiting,
        Ended,
        Disposed,
    }

    public class TimerComponent : EcsComponent
    {
        public List<long> TimeList = new();
        public Dictionary<long, object> Timers { get; set; } = new();
        //public Dictionary<long, Queue<ETTask>> SameTimers { get; set; } = new();

        //public List<long> FrameList = new();
        public Dictionary<int, FrameTimer> FrameTimers { get; set; } = new();
    }
}