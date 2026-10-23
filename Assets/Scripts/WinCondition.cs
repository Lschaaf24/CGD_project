using System;
using UnityEngine;
using UnityEngine.Events;

public class WinCondition : MonoBehaviour
{
    UnityEvent QuotaReached;
    UnityEvent TimeLimitReached;

    private 


    void Start()
    {
        if (QuotaReached == null)
        {
            QuotaReached = new UnityEvent();
        }

        QuotaReached.AddListener(OnQuotaReached);

        if (TimeLimitReached == null)
        {
            TimeLimitReached = new UnityEvent();
        }

        TimeLimitReached.AddListener(OnTimeLimitReached);
    }

    private void OnTimeLimitReached()
    {
        Debug.Log("Time Limit Reached");
    }

    private void OnQuotaReached()
    {
        Debug.Log("Quota Reached");

    }

    void Update()
    {
        
    }
}
