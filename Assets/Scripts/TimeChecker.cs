using UnityEngine;
using System;
using TMPro;

public class TimeChecker : MonoBehaviour
{
    public static TimeChecker Instance;

    public event Action OnTimeLimitReached;

    [Header("Debug")]
    [SerializeField] private bool TimerOff = false;

    [Header("Trackers")]
    [SerializeField] private float timeLimit;
    [SerializeField] private TMP_Text timerText;
    private bool expired = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeLimit = GameManager.instance.GetLevelDataSO().getTimeLimit();
    }

    // Update is called once per frame
    void Update()
    {

        if(TimerOff == true)
        {
            timeLimit -= Time.deltaTime;
            timerText.text = timeLimit.ToString(".0");
        }

        if (timeLimit <= 0 && expired == false)
        {
            expired = true;

            OnTimeLimitReached?.Invoke();
        }

    }

    public bool GetTimerOff()
    {
        return TimerOff;
    }

    public void SetTimerOff(bool b)
    {
        TimerOff = b;
    }
}
