using DG.Tweening;
using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class PlayerInputComponent : EcsComponent
    {
        public Tweener MoveTweener { get; set; }
        public Tweener LookAtTweener { get; set; }
    }
}