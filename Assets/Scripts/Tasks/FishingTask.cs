using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FMODUnity;


public class FishingTask: Task
{
    [SerializeField] float reelPercentage = 20;
    [SerializeField] Slider reelUI;
    [SerializeField] private EventReference fishPickupSound;

    bool reeling = false;


    private void Awake()
    {
        reelUI.value = reelPercentage/100;
      

    }
    protected override void Update()
    {
        base.Update();

        if (!reeling) return;

        if(reelPercentage >= 10.0f * playersRequired * Time.deltaTime) 
        {
            reelPercentage -= 10.0f * playersRequired * Time.deltaTime;
        }
        reelUI.value = reelPercentage/100;
    }

    protected override void EndTask() 
    {
        base.EndTask();
        reelUI.gameObject.SetActive(false);
        reelPercentage = 20;
        reeling = false;
    }

    protected override void BeginTask() 
    {
        base.BeginTask();
        reeling = true;
        reelUI.gameObject.SetActive(true); 
           RuntimeManager.PlayOneShot(fishPickupSound, transform.position);
        SoundManager.PlaySound(SoundType.FishPickup);
    }

    public override void TaskAction() 
    {
        if (!reeling) return;
        reelPercentage += 5.0f;
        if(reelPercentage >= 100) 
        {
            EndTask();
            RuntimeManager.PlayOneShot(fishPickupSound, transform.position);
          
        }
    }


}
