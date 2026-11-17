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
    [SerializeField] float reelGain = 7.5f;
    [SerializeField] float minInterval = 1.0f;
    [SerializeField] float maxInterval = 9.0f;
    [SerializeField] float minLossRate = 8.0f;
    [SerializeField] float maxLossRate = 12.0f;
    [SerializeField] float pulseSpeed = 500.0f;
    [SerializeField] GameObject waitingIcon;
    float lossRate = 10.0f;

    [SerializeField] Slider reelUI;
    bool reeling = false;

    float difference = 0;

    float lastInputTime = 0;
    float startTime = 0;

    int inputCount = 0;

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
                if (readyIcon) 
                {
                    readyIcon.SetActive(true);
                    waitingIcon.SetActive(false);
                }
                EventManager.Instance.ChangeCounter(EventManager.SceneCounter.ReadyRods, 1);
            }
        }

        if (!reeling) return;

        float inputsPerSecond = inputCount / (Time.time - startTime);
        Debug.Log(inputsPerSecond);
        float lossValue = lossRate * Mathf.Lerp(2f, 0.8f, inputsPerSecond / 6) * playersRequired * Time.deltaTime; //The faster the player presses, the easier it is to reel
        if (reelPercentage >= lossValue) 
        {
            reelPercentage -= lossValue;
        }
        difference =  (reelPercentage / 100) - reelUI.value;
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
        inputCount = 0;
        interval = UnityEngine.Random.Range(minInterval, maxInterval);
        waitingIcon.SetActive(true);
        particleSystem.Play();
    }

    protected override void BeginTask() 
    {
        base.BeginTask();
        reeling = true;
        reelUI.gameObject.SetActive(true);
        lossRate = UnityEngine.Random.Range(minLossRate, maxLossRate);
        startTime = Time.time;
    }

    public override void TaskAction() 
    {
        if (!reeling) return;
        reelPercentage += reelGain;
        inputCount++;
        lastInputTime = Time.time;
        if(reelPercentage >= 100) 
        {
            isCompleted = true;
            EndTask();
        }
    }

    public override void LeaveTask(PlayerController player)
    {
        base.LeaveTask(player);
        if (reeling) 
        {
            inputCount = 0;
            reeling = false;
            reelUI.gameObject.SetActive(false);
            if (readyIcon) 
            {
                readyIcon.gameObject.SetActive(true);
            }

            reelPercentage = startReelPercentage;
        }

    }

    public void PulseIconScale(GameObject handle) 
    {
        if (!reeling) return;

        float frameError = Time.deltaTime * pulseSpeed;
        if(handle.transform.localScale.x > 2) 
        {
            handle.transform.localScale -= (Vector3.one / 20) * frameError;
        }
        else if (difference > 0) 
        {
            handle.transform.localScale += (Vector3.one / 1.6f) * frameError;
        }
        else
        {
            Debug.Log("Error: " + frameError);
            float smooth = Mathf.Lerp(0, 1, (Time.time - lastInputTime) / 0.8f);

            float offsetX = Mathf.Sin(reelPercentage * 180f * Mathf.Deg2Rad) * smooth;
            float angleZ = Mathf.Sin(reelPercentage * 90f * Mathf.Deg2Rad) * 10f * smooth;

            RectTransform rect = handle.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(offsetX, rect.anchoredPosition.y);
            handle.transform.eulerAngles = new Vector3(0f, 0f, angleZ);
        }
        handle.transform.localScale = Vector3.one * Mathf.Clamp(handle.transform.localScale.x, 2, 3);
    }

    


}
