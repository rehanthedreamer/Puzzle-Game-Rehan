using UnityEngine;
using System;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public static TimerManager instance;
    public static event Action<string> OnTimerUpdated;
    public static event Action OnTimerFinished;
    public static event Action OnTimerStart;
    public static event Action onClickGamePlay;

    [Header("Timer Settings")]
    public float startTime = 120f; 
    private float currentTime;
    private bool isRunning = false;

    void Awake()
    {
      if(instance == null)
        instance = this;  
    }

    void OnEnable()
    {
        onClickGamePlay += OnClikcStartTimer;
    }
    void OnDisable()
    {
        onClickGamePlay -= OnClikcStartTimer;
    }

    public bool GetIsTimeRunning()
    {
        return isRunning;
    }
// start timer when user tap on play button 
    public void OnClikcStartTimer()
    {
        ResetTimer();
        StartTimer();
        OnTimerStart?.Invoke();
    }

// check active timer 
    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            OnTimerFinished?.Invoke();
        }

        OnTimerUpdated?.Invoke(FormatTime(currentTime));
    }

    // Format time in MM:SS
    public static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    public void StartTimer()
    {
        isRunning = true;
    }

// stop timer
    public void StopTimer()
    {
        isRunning = false;
        OnTimerFinished?.Invoke();
    }

// rest timer 
    public void ResetTimer()
    {
        currentTime = startTime;
        OnTimerUpdated?.Invoke(FormatTime(currentTime));
    }
}
