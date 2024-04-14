using UnityEngine;
using UnityEngine.Events;

public class SimpleTimer : MonoBehaviour
{
    public float interval = 10f;
    public bool loop = true;
    public UnityAction OnTimerElapsed;

    private bool _isStopped = true;
    private float _timeElapsed = 0;

    public void SetTimer(float interval, bool loop)
    { 
        this.interval = interval;
        this.loop = loop;
        _timeElapsed = 0f;
    }

    private void Update()
    {
        if (_isStopped) return;

        _timeElapsed += Time.deltaTime;

        if (_timeElapsed >= interval)
        {
            TimerEnded();
        }
    }

    public void StartTimer() => _isStopped = false;
    public void PauseTimer() => _isStopped = true;
        
    private void TimerEnded()
    {
        OnTimerElapsed.Invoke();

        if (loop)
        {
            _timeElapsed = 0f;
        }
        else
        {
            PauseTimer();
        }
    }
}
