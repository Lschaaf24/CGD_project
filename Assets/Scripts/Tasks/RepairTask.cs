using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RepairTask: Task
{
    Vector3 targetScale;
    private void OnEnable()
    {
        targetScale = transform.localScale;
        EventManager.Instance.ChangeCounter(EventManager.SceneCounter.RepairCount, 1);

        transform.position = new Vector3(transform.position.x, -6.776f, transform.position.z);


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
        if (bucket != null && !bucket.GetIsFull())
        {
            bucket.SetFill(true);
        }
        else { base.LeaveTask(engagedPlayers[0]); return; }
        EndTask();

    }

    public override void TaskAction() 
    {
        Debug.Log("CLEAN");
    }

    public void OnTriggerEnter(Collider collision)
    {
        Debug.Log("PLAYER SLIP");
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            //if(player.GetHeldItem())
            BucketItem bucket = player.GetHeldItem()?.GetComponent<BucketItem>();
            if (bucket != null && !bucket.GetIsFull()) return; 
            
            player.stunPlayer();
            
        }
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

