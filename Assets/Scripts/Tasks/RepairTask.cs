using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RepairTask: Task
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void EndTask() 
    {
        base.EndTask();
    }

    protected override void BeginTask() 
    {
        EndTask();

    }

    public override void TaskAction() 
    {
        Debug.Log("CLEAN");
    }
}
