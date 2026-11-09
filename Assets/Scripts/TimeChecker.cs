using UnityEngine;
using System;

public class TimeChecker : MonoBehaviour
{

    public event Action OnTimeLimitReached;
    [Header("Debug")]
    [SerializeField] private bool TimerOff = false;

    [Header("Trackers")]
    [SerializeField] private float timeLimit;
    private bool expired = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(TimerOff == true)
        {
            timeLimit -= Time.deltaTime;
        }

        if (timeLimit <= 0 && expired == false)
        {
            expired = true;

            OnTimeLimitReached?.Invoke();
        }

    }
}
