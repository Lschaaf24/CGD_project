using System;
using UnityEngine;

public class Quota : MonoBehaviour
{
    public event Action<Quota> OnCompleted;
    public event Action<Quota> OnFailed;
    public event Action<int, int> OnProgressChanged;

    public QuotaDataSO Data { get; private set; }
    private int progress;
    private float timeLeft;
    private bool finished;

    public float TimeLeft => timeLeft;

    public void Init(QuotaDataSO data)
    {
        Data = data;
        timeLeft = data.getTimeLimit();
    }

  /*  public void AddProgress(int amount)
    {
        progress += amount;
        if (progress >= Data.getTargetNum())
            OnCompleted?.Invoke(this);
         
    }*/

    public bool TryAddProgress(Item item)
    {
        if(finished) return false;
        
        float weight = item.GetWeight();
        if (weight <= Data.getWeightMin() || weight >= Data.getWeightMax()) return false;

        progress++;
        OnProgressChanged?.Invoke(progress, Data.getTargetNum());

        if(progress >= Data.getTargetNum())
        {
            finished = true;
            OnCompleted?.Invoke(this);
        }

        return true;
    
    }

    // Update is called once per frame
    void Update()
    {
        if (finished) return;

        timeLeft -= Time.deltaTime;
        if(timeLeft <= 0f)
        {
            finished = true;
            OnFailed?.Invoke(this);
        }
    }
}
