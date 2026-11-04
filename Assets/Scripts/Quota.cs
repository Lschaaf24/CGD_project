using System;
using System.Data.Common;
using UnityEngine;

public class Quota : MonoBehaviour
{
    public event Action<Quota> OnCompleted;
    public event Action<Quota> OnFailed;

    public QuotaDataSO Data { get; private set; }
    private int progress;
    private float timeLeft;


    public void Init(QuotaDataSO data)
    {
        Data = data;
        timeLeft = data.getTimeLimit();
    }

    public void AddProgress(int amount)
    {
        progress += amount;
        if (progress >= Data.getTargetNum())
            OnCompleted?.Invoke(this);
         
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if(timeLeft <= 0)
            OnFailed?.Invoke(this);
    }
}
