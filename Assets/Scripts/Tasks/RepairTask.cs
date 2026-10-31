using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RepairTask: Task
{

    private void OnEnable()
    {

        EventManager.Instance.ChangeCounter(EventManager.SceneCounter.RepairCount, 1);

    }

    private void OnDisable()
    {
        EventManager.Instance.ChangeCounter(EventManager.SceneCounter.RepairCount, -1);
    }

   

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
