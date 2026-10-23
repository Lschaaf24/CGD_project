using System;
using UnityEngine;
using UnityEngine.Events;

public class WinCondition : MonoBehaviour
{
    UnityEvent QuotaReached;
    UnityEvent TimeLimitReached;



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
        throw new NotImplementedException();
    }

    private void OnQuotaReached()
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        
    }
}
