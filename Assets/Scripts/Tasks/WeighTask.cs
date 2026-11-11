using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class WeighTask : Task
{
    [Header("Misc")]
    [SerializeField] TMP_Text massDisplay;
    [SerializeField] GameObject hookPoint;
    [SerializeField] GameObject hook;

    [SerializeField] Material correctLight;
    [SerializeField] Material wrongLight;
    [SerializeField] MeshRenderer lightMesh;

    float actualWeight = 0f;
    float duration = 1f;

    float targetWeight = 5.0f;

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

        StartCoroutine(ResetHook());


        base.EndTask();

    }

    protected override void BeginTask()
    {
        base.BeginTask();
        if (currentItem != null)
        {
            currentItem.transform.SetParent(hookPoint.transform, false);
            currentItem.GetComponent<Rigidbody>().isKinematic = true;
            currentItem.GetComponent<Collider>().enabled = false;
            actualWeight = currentItem.GetWeight();
            StartCoroutine(WeighFish());

            if(actualWeight > targetWeight) 
            {
                lightMesh.material = correctLight;
            }
            else 
            {
                lightMesh.material = wrongLight;
            }

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
        float currentWeight = 0f;

        while (timeElapsed < duration)
        {

            float weight = Mathf.SmoothStep(currentWeight, actualWeight, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            massDisplay.text = weight.ToString("0.00");
            hook.transform.localPosition = new Vector3(0, Mathf.SmoothStep(0, -0.5f, weight / 12.0f), 0);

            yield return null;
        }

        currentWeight = actualWeight;
        isCompleted = true;
        currentItem.SetHasBeenWeighed(true);
        EndTask();
    }

    IEnumerator ResetHook()
    {

        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            float weight = Mathf.SmoothStep(actualWeight, 0, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            massDisplay.text = weight.ToString("0.00");
            hook.transform.localPosition = new Vector3(0, Mathf.SmoothStep(0, -0.5f, weight / 12.0f), 0);

            if (timeElapsed / duration >= 0.1f  && currentItem != null)
            {
                EjectItem();
            }

            yield return null;
        }

        actualWeight = 0;
    }

    private void EjectItem() 
    {
        currentItem.transform.parent = null;
        currentItem.GetComponent<Collider>().enabled = true;
        currentItem.GetComponent<Rigidbody>().isKinematic = false;

        Vector3 ejectDirection = -transform.forward + (Vector3.up * 2);
        currentItem.GetComponent<Rigidbody>().AddForce(ejectDirection * 100.0f);
        currentItem = null;
    }
}