using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{ 

    [SerializeField] List<Task> taskList;
    List<Event> activeEvents;
    int handledEvents = 0;
    [SerializeField] float eventRate;
    float lastEventTime = 0.0f;

    int[,] eventMap = new int[30, 30]; 
    
    enum Event 
    {
        Tip,
        FishCatch,
        ReplaceBait,
        Repair
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
             
    }
}
