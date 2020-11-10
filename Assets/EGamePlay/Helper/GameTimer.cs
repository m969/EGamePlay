using System;

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
        _maxTime = maxTime;
        _time = 0f;
    }

    public void Reset()
    {
        _time = 0f;
    }

    public GameTimer UpdateAsFinish(float delta, Action action = null)
    {
        if (!IsFinished)
        {
            _time += delta;
            if (action != null)
            {
                _onFinish = action;
            }
            if (IsFinished)
            {
                _onFinish?.Invoke();
            }
        }
        return this;
    }

    public void UpdateAsRepeat(float delta, Action action = null)
    {
        _time += delta;
        if (action != null)
        {
            _onFinish = action;
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