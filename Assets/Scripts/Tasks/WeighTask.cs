using TMPro;
using Unity.Multiplayer.PlayMode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class WeighTask : Task
{
    [SerializeField] TMP_Text massDisplay;
    private void Awake()
    {
        reward = null;
    }
    protected override void Update()
    {
        taskReady = true;
    }

    protected override void EndTask()
    {
        base.EndTask();

        if (currentItem == null) return;
        if (!currentItem.GetHasBeenWeighed() && currentItem.GetItemType() == typeof(FishData)) 
        {
            currentItem.SetHasBeenWeighed(true);
            EventManager.Instance.ChangeCounter(EventManager.SceneCounter.WeighingRequired, -1);
        }

    }

    protected override void BeginTask()
    {
        base.BeginTask();
        if(currentItem != null)
        {
            string weight = currentItem.GetWeight().ToString("0.00");

            massDisplay.text = weight;


        }
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
