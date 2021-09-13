using System;

namespace GameUtils
{
#if NOT_UNITY
    public static class Time
    {
        public static long FrameEndTime;
        public static long FrameTime;
        public static float deltaTime { get; set; } = FrameTime / 1000f;
    }
#endif
    public class GameTimer
    {
        private float _maxTime;
        private float _time;
        private Action _onFinish;

        public bool IsFinished => _time >= _maxTime;
        public bool IsRunning => _time < _maxTime;

        public float Time => _time;

        public float MaxTime
        {
            get => _maxTime;
            set => _maxTime = value;
        }

        public GameTimer(float maxTime)
        {
            if (maxTime <= 0)
            {
                throw new Exception($"_maxTime can not be 0 or negative");
            }
            _maxTime = maxTime;
            _time = 0f;
        }

        public void Reset()
        {
            _time = 0f;
        }

        public GameTimer UpdateAsFinish(float delta, Action onFinish = null)
        {
            if (!IsFinished)
            {
                _time += delta;
                if (onFinish != _onFinish)
                {
                    _onFinish = onFinish;
                }
                if (IsFinished)
                {
                    _onFinish?.Invoke();
                }
            }
            return this;
        }

        public void UpdateAsRepeat(float delta, Action onRepeat = null)
        {
            if (delta > _maxTime)
            {
                throw new Exception($"_maxTime too small, delta:{delta} > _maxTime:{_maxTime}");
            }
            _time += delta;
            if (onRepeat != _onFinish)
            {
                _onFinish = onRepeat;
            }
            while (_time >= _maxTime)
            {
                _time -= _maxTime;
                _onFinish?.Invoke();
            }
        }

        public void OnFinish(Action onFinish)
        {
            _onFinish = onFinish;
        }

        public void OnRepeat(Action onRepeat)
        {
            _onFinish = onRepeat;
        }
    }
}