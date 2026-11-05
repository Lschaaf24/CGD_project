using System.Collections;
using TMPro;
using Unity.Multiplayer.PlayMode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class WeighTask : Task
{
    [Header("Misc")]
    [SerializeField] TMP_Text massDisplay;
    [SerializeField] GameObject hook;

    float currentWeight = 0f;
    float actualWeight = 0f;
    float duration = 0.5f;

    PlayerController interactedPlayer;

    private void Awake()
    {
        interval = 0;
        isCompleted = false;
        reward = null;
    }
 
    protected override void EndTask()
    {
        EventManager.Instance.ChangeCounter(EventManager.SceneCounter.WeighingRequired, -1);
        EventManager.Instance.ChangeCounter(EventManager.SceneCounter.SortingRequired, 1);
        Debug.LogWarning(currentItem.name);

        currentItem.transform.parent = null;
        currentItem.GetComponent<Collider>().enabled = true;

        if(interactedPlayer != null) 
        {
            Vector3 ejectDirection = (transform.position - interactedPlayer.transform.position).normalized;
            currentItem.GetComponent<Rigidbody>().AddForce(ejectDirection * 100.0f);
        }
        currentItem = null;



        base.EndTask();

    }

    protected override void BeginTask()
    {
        base.BeginTask();
        if(currentItem != null)
        {
            currentItem.transform.SetParent(hook.transform);
            actualWeight = currentItem.GetWeight();
            StartCoroutine(WeighFish());
            interactedPlayer = engagedPlayers[0];
        }
    }

    public override void TaskAction()
    {

    }

    public override void LeaveTask(PlayerController player)
    {
        interactedPlayer = null;
    }

    IEnumerator WeighFish()
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            string weight = Mathf.SmoothStep(currentWeight, actualWeight, timeElapsed / duration).ToString("0.00");
            timeElapsed += Time.deltaTime;
            massDisplay.text = weight;


            yield return null;
        }

        currentWeight = actualWeight;
        isCompleted = true;
        currentItem.SetHasBeenWeighed(true);
        EndTask();
    }




}
