using ECS;
using EGamePlay.Combat;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public struct BeforeRunEventCmd : ICommand
    {
        public EcsEntity Entity {  get; set; }
        public IEventRun EventRun;
        public object EventEntity;
        public object EventArgs2;
        public object EventArgs3;
    }

    public struct AfterRunEventCmd : ICommand
    {
        public EcsEntity Entity {  get; set; }
        public IEventRun EventRun;
        public object EventEntity;
        public object EventArgs2;
        public object EventArgs3;
    }
}

//namespace ECSGame
//{
//    public struct EntityUpdateCmd : ICommand
//    {
//        public EcsEntity Entity {  get; set; }
//        public EcsComponent ChangeComponent;
//        public object Args;
//    }

//    public struct EntityCreateCmd : ICommand
//    {
//        public EcsEntity Entity {  get; set; }
//    }

//    public struct EntityDestroyCmd : ICommand
//    {
//        public EcsEntity Entity {  get; set; }
//    }
//}
