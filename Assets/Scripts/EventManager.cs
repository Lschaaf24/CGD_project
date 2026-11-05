
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{ 
    public static EventManager Instance;
    float targetIntensity = 0;
    float currentIntensity = 0;





    [SerializeField] List<GameEvent> events = new();

    public enum SceneCounter
    {
        ReadyRods,
        WeighingRequired,
        SortingRequired,
        BaitReplacementsRequired,
        RepairCount,
        TipDegrees,
        ActiveMajorEvents
    }

    float targetWeight = 100.0f;
    float expectedWeight = 0;
    float timeLimit = 180; //In Seconds
    float elapsedTime = 0.0f;

    int currentBudget = 2;
    int maxBudget = 2;

    float budgetRegenTimer;

    private Dictionary<SceneCounter, int> counters = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CheckEvents());
    }

    private void Awake()
    {
        Instance = this;
        foreach (SceneCounter counter in System.Enum.GetValues(typeof(SceneCounter))) 
        {
            counters[counter] = 0;
        }
    }

   


    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        expectedWeight = targetWeight * (elapsedTime / timeLimit);

        float pressure = CalculatePressure();

        targetIntensity = Mathf.Clamp01(pressure / 100f);

        float changeSpeed = targetIntensity > currentIntensity ? 0.15f : 0.05f;

        currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, Time.deltaTime * changeSpeed);



        UpdateBudget();
    }

    public void ChangeCounter(SceneCounter counter, int amount) 
    {
        if (counters[counter] + amount < 0)
        {
            return;
        }
        counters[counter] += amount;
    }

    public int GetCounter(SceneCounter counter)
    {
        return counters[counter];
    }

    private IEnumerator CheckEvents()
    {
        while (true)
        {

            yield return new WaitForSeconds(5);


            List<GameEvent> elibgibleEvents = new();

            int totalWeight = 0;

            foreach (GameEvent gameEvent in events)
            {
                if (gameEvent.isActive) continue;
                if (currentIntensity < gameEvent.GetRequiredIntensity()) continue;
                if (gameEvent.IsCoolingDown()) continue;
                if (!CanAffordEvent(gameEvent)) continue;

                elibgibleEvents.Add(gameEvent);
                totalWeight += gameEvent.baseWeight;
            }

            if (elibgibleEvents.Count == 0) continue;

            int randomValue = UnityEngine.Random.Range(0, totalWeight);

            foreach (GameEvent gameEvent in elibgibleEvents)
            {
                randomValue -= gameEvent.baseWeight;

                if (randomValue < 0)
                {
                    TriggerEvent(gameEvent);
                    break;
                }
            }
        }
    }


    private float CalculatePressure() 
    {
        float pressure = 0;
        pressure += counters[SceneCounter.ReadyRods] * 8;
        pressure += counters[SceneCounter.WeighingRequired] * 5;
        pressure += counters[SceneCounter.SortingRequired] * 4;
        pressure += counters[SceneCounter.BaitReplacementsRequired] * 6;
        pressure += counters[SceneCounter.RepairCount] * 5;
        pressure += counters[SceneCounter.TipDegrees] * 12;

        return pressure;
    }

    private void UpdateBudget() 
    {

        if (currentIntensity <= 0.2)
        {
            maxBudget = 2;
        }
        else if (currentIntensity <= 0.5)
        {
            maxBudget = 4;
        }
        else if (currentIntensity <= 0.8)
        {
            maxBudget = 6;
        }
        else
        {
            maxBudget = 8;
        }

        currentBudget = Mathf.Min(currentBudget, maxBudget);

        //float budgetRegenTime = Mathf.Lerp(maxBudgetRegenTime, minBudgetRegenTime, currentIntensity);

        if(currentBudget < maxBudget) 
        {
            budgetRegenTimer += Time.deltaTime;

            if (budgetRegenTimer >= 15.0f)
            {
                currentBudget++;

                budgetRegenTimer -= 15;
            }
        }
        else 
        {
            budgetRegenTimer = 0;
        }

    }

    private bool CanAffordEvent(GameEvent gameEvent) 
    {
        return currentBudget >= gameEvent.budgetCost;
    }


    private void TriggerEvent(GameEvent gameEvent) 
    {
        if (!CanAffordEvent(gameEvent)) return;
        if (gameEvent.IsCoolingDown()) return;

        currentBudget -= gameEvent.budgetCost;

        gameEvent.lastTriggeredTime = Time.time;
        gameEvent.StartEvent();
    }



}
