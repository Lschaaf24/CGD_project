using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FishingTask: Task
{
    [SerializeField] float reelPercentage = 20;
    [SerializeField] Slider reelUI;
    bool reeling = false;

    public static event Action OnRodReady;
    public static event Action OnRodReeledIn;


    private void Awake()
    {
        reelUI.value = reelPercentage/100;
    }
    protected override void Update()
    {
        if (taskReady == false && persistentTask && isCompleted == true)
        {
            elapsed_time += Time.deltaTime;
            if (elapsed_time >= interval)
            {
                elapsed_time = 0.0f;
                taskReady = true;
                icon.SetActive(true);
                EventManager.Instance.ChangeCounter(EventManager.SceneCounter.ReadyRods, 1);
            }
        }

        if (!reeling) return;

        if(reelPercentage >= 10.0f * playersRequired * Time.deltaTime) 
        {
            reelPercentage -= 10.0f * playersRequired * Time.deltaTime;
        }
        reelUI.value = reelPercentage/100;
    }

    protected override void EndTask() 
    {
        base.EndTask();
        EventManager.Instance.ChangeCounter(EventManager.SceneCounter.ReadyRods, -1);
        reelUI.gameObject.SetActive(false);
        reelPercentage = 20;
        reeling = false;
    }

    protected override void BeginTask() 
    {
        base.BeginTask();
        reeling = true;
        reelUI.gameObject.SetActive(true); 
    }

    public override void TaskAction() 
    {
        if (!reeling) return;
        reelPercentage += 5.0f;
        if(reelPercentage >= 100) 
        {
            EndTask();
        }
    }


}
