using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{ 
    public static EventManager Instance;
    [SerializeField] List<GameObject> taskList;
    [SerializeField] List<Event> activeEvents;
    int handledEvents = 0;
    [SerializeField] float eventRate;
    float lastEventTime = 0.0f;

    int pressure = 0;
    int intensity = 1 ;
    
    struct Event 
    {
        string name;
        int baseWeight;
        int maxIntensity;
        float cooldown;
        float duration;
        string[] excludeEvents;
        int intesity;
    }

    public enum SceneCounter
    {
        ReadyRods,
        WeighingRequired,
        BaitReplacementsRequired,
        PuddleCount,
        SlidingBoxes,
        ActiveMajorEvents
    }

    float targetWeight = 100.0f;
    float expectedWeight = 0;
    float timeLimit = 180; //In Seconds
    float elapsedTime = 0.0f;

    private Dictionary<SceneCounter, int> counters = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        taskList = GameObject.FindGameObjectsWithTag("Event").ToList<GameObject>();
        Instance = this;
    }

    private void Awake()
    {
        foreach(SceneCounter counter in System.Enum.GetValues(typeof(SceneCounter))) 
        {
            counters[counter] = 0;
        }
    }

   


    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        expectedWeight = targetWeight * (elapsedTime / timeLimit);

        CalculatePressure();
        Debug.Log(pressure);

    }

    public void ChangeCounter(SceneCounter counter, int amount) 
    {
        counters[counter] += amount;

        CheckEvents();
    }

    public int GetCounter(SceneCounter counter)
    {
        return counters[counter];
    }

    private void CheckEvents()
    {
        if (GetCounter(SceneCounter.ReadyRods) >= 3)
        {
            Debug.Log("Three rods ready!");
        }
    }


    private void CalculatePressure() 
    {
        pressure = 0;
        pressure += counters[SceneCounter.ReadyRods] * 8;
        pressure += counters[SceneCounter.WeighingRequired] * 5;
        pressure += counters[SceneCounter.BaitReplacementsRequired] * 4;
        pressure += counters[SceneCounter.PuddleCount] * 5;
        pressure += counters[SceneCounter.SlidingBoxes] * 12;
        pressure += counters[SceneCounter.ActiveMajorEvents] * 15;
    }


}
