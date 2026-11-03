using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RepairEvent : GameEvent
{
    [SerializeField] private GameObject repairTask;

    private List<Task> tasks = new();
    
    public override void StartEvent()
    {
        GameObject taskObject = Instantiate(repairTask);
        Task task = taskObject.GetComponent<Task>();

        if (task == null) 
        {
            EndEvent();
            return;
        }

        task.OnTaskCompleted += HandleTaskCompleted;
        tasks.Add(task);

        intensityModifier += 0.1f;

    }

    private void HandleTaskCompleted(Task task) 
    {
        task.OnTaskCompleted -= HandleTaskCompleted;
        tasks.Remove(task);

        intensityModifier -= 0.1f;
    }

    public override void EndEvent()
    {
      
    }
}
