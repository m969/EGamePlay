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

    public enum SpeedType
    {
        Speed,
        Duration,
    }

    public class MoveWithDotweenComponent : Component
    {
        public SpeedType SpeedType { get; set; }
        public float Speed { get; set; }
        public float Duration { get; set; }
        public IPosition PositionEntity { get; set; }
        public IPosition TargetPositionEntity { get; set; }
        public Vector3 Destination { get; set; }
        public Tweener MoveTweener { get; set; }
        private System.Action MoveFinishAction { get; set; }


        public override void Awake()
        {
            PositionEntity = (IPosition)Entity;
        }

        public override void Update()
        {
            if (TargetPositionEntity != null)
            {
                if (SpeedType == SpeedType.Speed) DoMoveToWithSpeed(TargetPositionEntity, Speed);
                if (SpeedType == SpeedType.Duration) DoMoveToWithTime(TargetPositionEntity, Duration);
            }
        }

        public MoveWithDotweenComponent DoMoveTo(Vector3 destination, float duration)
        {
            Destination = destination;
            DOTween.To(()=> { return PositionEntity.Position; }, (x) => PositionEntity.Position = x, Destination, duration).SetEase(Ease.Linear).OnComplete(OnMoveFinish);
            return this;
        }

        public void DoMoveToWithSpeed(IPosition targetPositionEntity, float speed = 1f)
        {
            Speed = speed;
            SpeedType = SpeedType.Speed;
            TargetPositionEntity = targetPositionEntity;
            MoveTweener?.Kill();
            var dist = Vector3.Distance(PositionEntity.Position, TargetPositionEntity.Position);
            var duration = dist / speed;
            MoveTweener = DOTween.To(() => { return PositionEntity.Position; }, (x) => PositionEntity.Position = x, TargetPositionEntity.Position, duration);
        }

        public void DoMoveToWithTime(IPosition targetPositionEntity, float time = 1f)
        {
            Duration = time;
            SpeedType = SpeedType.Duration;
            TargetPositionEntity = targetPositionEntity;
            MoveTweener?.Kill();
            MoveTweener = DOTween.To(() => { return PositionEntity.Position; }, (x) => PositionEntity.Position = x, TargetPositionEntity.Position, time);
        }

        //public void DoMovePath(List<Vector3> points)
        //{
        //    TargetPositionEntity = targetPositionEntity;
        //    MoveTweener?.Kill();
        //    var dist = Vector3.Distance(PositionEntity.Position, TargetPositionEntity.Position);
        //    var duration = dist / 10;
        //    MoveTweener = DOTween.To(() => { return PositionEntity.Position; }, (x) => PositionEntity.Position = x, TargetPositionEntity.Position, duration);
        //}

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