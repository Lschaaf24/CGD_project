using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class FishingTask: Task
{
    float reelPercentage = 20;
    [SerializeField] float startReelPercentage = 20;
    [SerializeField] Slider reelUI;
    bool reeling = false;

    private void Awake()
    {
        reelUI.value = reelPercentage/100;
        interval += UnityEngine.Random.Range(-4, 4);
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
        EventManager.Instance.ChangeCounter(EventManager.SceneCounter.WeighingRequired, 1);
        reelUI.gameObject.SetActive(false);
        reelPercentage = startReelPercentage;
        reeling = false;
        interval = 5 + UnityEngine.Random.Range(-4, 4);
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

    public override void LeaveTask(PlayerController player)
    {
        base.LeaveTask(player);
        if (reeling) 
        {
            reeling = false;
            reelUI.gameObject.SetActive(false);
            icon.gameObject.SetActive(true);
            reelPercentage = startReelPercentage;
        }

    }


}
