using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{ 

    [SerializeField] List<GameObject> taskList;
    List<Event> activeEvents;
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

    int readyRods = 0;
    int weighingRequired = 0;
    int baitReplacementsRequired = 0;
    int puddleCount = 0;
    int slidingBoxes = 0;
    int activeMajorEvents = 0;

    float targetWeight = 100.0f;
    float expectedWeight = 0;
    float timeLimit = 180; //In Seconds
    float elapsedTime = 0.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        taskList = GameObject.FindGameObjectsWithTag("Event").ToList<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        expectedWeight = targetWeight * (elapsedTime / timeLimit);

    }

    private void CalculatePressure() 
    {
        pressure = 0;

        pressure += readyRods * 8;
        pressure += weighingRequired * 5;
        pressure += baitReplacementsRequired * 4;
        pressure += puddleCount * 5;
        pressure += slidingBoxes * 12;
        pressure += activeMajorEvents * 15;
    }
}
