using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
using DG.Tweening;

namespace EGamePlay.Combat
{
    public enum MoveType
    {
        TargetMove,
        PathMove,
    }

    public class MoveWithDotweenComponent : Component
    {
        public IPosition PositionEntity { get; set; }
        public IPosition TargetPositionEntity { get; set; }
        public Vector3 Destination { get; set; }
        public Tweener MoveTweener { get; set; }
        private System.Action MoveFinishAction { get; set; }


        public override void Setup()
        {
            PositionEntity = (IPosition)Entity;
        }

        public override void Update()
        {
            if (TargetPositionEntity != null)
            {
                DoMoveTo(TargetPositionEntity);
            }
        }

        public MoveWithDotweenComponent DoMoveTo(Vector3 destination, float duration)
        {
            Destination = destination;
            DOTween.To(()=> { return PositionEntity.Position; }, (x) => PositionEntity.Position = x, Destination, duration).SetEase(Ease.Linear).OnComplete(OnMoveFinish);
            return this;
        }

        public void DoMoveTo(IPosition targetPositionEntity)
        {
            TargetPositionEntity = targetPositionEntity;
            MoveTweener?.Kill();
            var dist = Vector3.Distance(PositionEntity.Position, TargetPositionEntity.Position);
            var duration = dist / 10;
            MoveTweener = DOTween.To(() => { return PositionEntity.Position; }, (x) => PositionEntity.Position = x, TargetPositionEntity.Position, duration);
        }

        public void OnMoveFinish(System.Action action)
        {
            MoveFinishAction = action;
        }

        private void OnMoveFinish()
        {
            MoveFinishAction?.Invoke();
        }
    }
}