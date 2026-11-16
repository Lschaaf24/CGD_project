using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RepairEvent : GameEvent
{
    [SerializeField] private GameObject repairTask;

    [SerializeField] List<Transform> spawnPoints = new();
    
    private Dictionary<Task, Transform> activeTaskPoints = new();
    
    
    public override void StartEvent()
    {
        if (spawnPoints.Count <= 0) return;
        
            GameObject taskObject = Instantiate(repairTask);
            int i = UnityEngine.Random.Range(0, spawnPoints.Count - 1);
            Transform spawnPoint = spawnPoints[i];
            taskObject.transform.position = spawnPoint.position;
            Task task = taskObject.GetComponent<Task>();
            activeTaskPoints[task] = spawnPoint;
            spawnPoints.Remove(spawnPoint);
        


        if (task == null) 
        {
            EndEvent();
            return;
        }

        task.OnTaskCompleted += HandleTaskCompleted;

        intensityModifier += 0.1f;

    }

    private void HandleTaskCompleted(Task task) 
    {
        task.OnTaskCompleted -= HandleTaskCompleted;
        spawnPoints.Add(activeTaskPoints[task]);

        intensityModifier -= 0.1f;
    }

    public override void EndEvent()
    {
      
    }
}
