using Unity.Multiplayer.PlayMode;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class WeighTask : Task
{
    private void Awake()
    {
        reward = null;
    }
    protected override void Update()
    {

    }

    protected override void EndTask()
    {
        base.EndTask();
        taskReady = true;
    }

    protected override void BeginTask()
    {
        base.BeginTask();
        EndTask();
    }

    public override void TaskAction()
    {
        //if (!reeling) return;
        //reelPercentage += 5.0f;
        //if (reelPercentage >= 100)
        //{
        //    EndTask();

    }


}
