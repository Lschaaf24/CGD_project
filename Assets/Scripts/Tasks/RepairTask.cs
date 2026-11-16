using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RepairTask: Task
{
    Vector3 targetScale;
    private void OnEnable()
    {
        targetScale = transform.localScale;
        EventManager.Instance.ChangeCounter(EventManager.SceneCounter.RepairCount, 1);
        StartCoroutine(LerpIn());

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
        BucketItem bucket = engagedPlayers[0].GetHeldItem().GetComponent<BucketItem>();
        if (bucket != null)
        {
            bucket.SetFill(true);
        }
        EndTask();

    }

    public override void TaskAction() 
    {
        Debug.Log("CLEAN");
    }

    IEnumerator LerpIn() 
    {
        float timeElapsed = 0;

        while (timeElapsed < 1.0f)
        {

            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, timeElapsed / 1.0f);
            timeElapsed += Time.deltaTime;
          
            yield return null;
        }

        transform.localScale = targetScale;
    }
}

