using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Task : MonoBehaviour
{
   
   
    [Header("Task Stats")]


    [Tooltip("How long the player has to complete the task in seconds")]
    [Range(1, 20)]
    [SerializeField] private float timeLimit = 1.0f;

    [Tooltip("The score the player will achieve from completing the task")]
    [SerializeField] private uint scoreAddition = 0;


    [Tooltip("The number of players required to complete the task")]
    [Range(1, 4)]
    [SerializeField] protected int playersRequired = 1;

    [Tooltip("Will the task always persist or does it spawn temporarily")]
    [SerializeField] protected bool persistentTask = false;

    [Tooltip("Does the task require continous input?")]
    [SerializeField] private bool continuousTask = false;

    [Tooltip("The item the player gains from completing the task")]
    [SerializeField] protected GameObject reward;
    [SerializeField] bool ejectItem;

    [SerializeField] private Transform ejectPoint;
    [SerializeField] private Transform peakPoint;
    [SerializeField] private Transform targetPoint;

    [SerializeField] private ItemData requiredItem;


    [Tooltip("Will the task take the players item?")]
    [SerializeField] bool takeItem = false;

    [Space(15)]

    [Header("FX")]

    [Tooltip("Animation to be played on the player when interacting with the task")]
    [SerializeField] private Animation animation;

    [Tooltip("Sound Effect to be played when the player completes the task")]
    [SerializeField] private AudioClip soundEffect;

    [Tooltip("Particle Effect to be played when the task is ready")]
    [SerializeField] protected ParticleSystem particleSystem;

    [Tooltip("Icon that appears when the task is ready")]
    [SerializeField] protected GameObject readyIcon;

    protected bool isCompleted = true;

    int currentPlayers = 0;
    bool taskStarted = false;
    [SerializeField] protected bool taskReady = false;

    protected List<PlayerController> engagedPlayers = new List<PlayerController>();

    protected float interval = 5.0f;
    protected float elapsed_time = 0.0f;

    protected Item currentItem = null;

    public event Action<Task> OnTaskCompleted;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (!persistentTask) 
        {
            taskReady = true; 
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
        if (taskReady == false && persistentTask && isCompleted == true) 
        {
            elapsed_time += Time.deltaTime;
            if (elapsed_time >= interval) 
            {
                elapsed_time = 0.0f;
                taskReady = true;
                if(readyIcon  != null)
                {
                    readyIcon.SetActive(true);
                }
            }
        }
    }

    public bool EnterTask(PlayerController player) 
    {
        if (!taskReady) return false;

        Item heldItem = player.GetHeldItem();
        if(requiredItem != null) 
        {
            if(heldItem == null || heldItem.GetItemData() != requiredItem) 
            {
                return false;
            }
        }
        else if (heldItem != null && !takeItem)
        {
            return false;
        }

        if (takeItem)
        {
            Debug.Log("current item:" + currentItem);
            currentItem = player.SwapItem(currentItem, base.transform);
           
            if (currentItem == null) return false;
        }


        Debug.Log("ENTERED");
        if(currentPlayers < playersRequired) 
        {
            engagedPlayers.Add(player);
            currentPlayers++;
            if(currentPlayers == playersRequired) 
            {
                BeginTask();
            }
            return true;
        }
        return false;
    }
    protected virtual void EndTask()
    {
        Debug.Log("TASK COMPLETED");
        if(reward != null) 
        {
            Vector3 ejectDirection = Vector3.zero;//(transform.up * 10) - (transform.forward * 3);
            GameObject item = Instantiate(reward, null);
            if (ejectItem) 
            {
                item.transform.position = ejectPoint.position;
                StartCoroutine(EjectItem(item));
            }
            else
            {
                engagedPlayers[0].ReceiveItem(item.GetComponent<Item>());
            }
        }
        foreach (var player in engagedPlayers)
        {
            player.TaskEject();
        }
        taskReady = false;

        if (persistentTask) 
        {
            currentPlayers = 0;
            engagedPlayers.Clear();
        }
        else 
        {
            Destroy(readyIcon);
            Destroy(this.gameObject);
        }
        OnTaskCompleted?.Invoke(this);
    }



    public virtual void TaskAction() { }

    protected virtual void BeginTask()  
    {
        if (readyIcon) 
        {
            readyIcon.SetActive(false);
        }
        taskReady = false;
        isCompleted = false;
    }

    public bool isTaskCompleted() 
    {
        if(isCompleted) return true;
        return false;
    }

    public virtual void LeaveTask(PlayerController player) 
    {
        player.TaskEject();
        currentPlayers -= 1;
        engagedPlayers.Remove(player);
       
        taskReady = true;
        Debug.Log("FAILED TASK");
    }

    IEnumerator EjectItem(GameObject item) 
    {
        float timeElapsed = 0;
        float duration = 1;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            item.transform.position = Bezier(ejectPoint.position, peakPoint.position,targetPoint.position, timeElapsed / duration); 
            yield return null;
        }

        item.transform.position = targetPoint.position;
        
    }


    Vector3 Bezier(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a, b, t);
    }

    Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return Vector3.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
    }

    Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        return Vector3.Lerp(Bezier(a, b, c, t), Bezier(b, c, d, t), t);
    }






}
