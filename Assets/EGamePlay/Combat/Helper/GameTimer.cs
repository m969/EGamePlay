using System;

public class GameTimer
{
    private float _maxTime;
    private float _time;
    private Action _onFinish;

    public bool IsFinished => _time >= _maxTime;

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

    public GameTimer UpdateAsFinish(float delta)
    {
        _time += delta;
        if (IsFinished)
        {
            _onFinish?.Invoke();
        }
        return this;
    }

    public void UpdateAsRepeat(float delta)
    {
        _time += delta;
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