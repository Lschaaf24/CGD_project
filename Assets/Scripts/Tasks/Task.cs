using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private bool persistentTask = false;

    [Tooltip("Does the task require continous input?")]
    [SerializeField] private bool continuousTask = false;

    [Space(15)]

    [Header("FX")]

    [Tooltip("Animation to be played on the player when interacting with the task")]
    [SerializeField] private Animation animation;

    [Tooltip("Sound Effect to be played when the player completes the task")]
    [SerializeField] private AudioClip soundEffect;

    [Tooltip("Particle Effect to be played when the task is ready")]
    [SerializeField] private ParticleSystem particleSystem;

    [Tooltip("Icon that appears when the task is ready")]
    [SerializeField] private GameObject icon;

    bool isCompleted = false;
    int currentPlayers = 0;
    bool taskStarted = false;
    bool taskReady = false;

    List<PlayerController> engaged_players = new List<PlayerController>();

    float interval = 5.0f;
    float elapsed_time = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    
        if (taskReady == false) 
        {
            elapsed_time += Time.deltaTime;
            if (elapsed_time >= interval) 
            {
                taskReady = true;
                elapsed_time = 0.0f;
                icon.SetActive(true);
            }
        }   
    }

    public bool EnterTask(PlayerController player) 
    {
        if (!taskReady) return false;
        Debug.Log("ENTERED");
        if(currentPlayers < playersRequired) 
        {
            engaged_players.Add(player);
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
        if (persistentTask) 
        {
            taskReady = false;
            isCompleted = true;
            currentPlayers = 0;
            foreach(var player in engaged_players) 
            {
                player.TaskEject();
            }
            engaged_players.Clear();
        }
        else 
        {
            Destroy(this.gameObject);
        }
    }



    public virtual void TaskAction() { }

    protected virtual void BeginTask()  
    { 
        icon.SetActive(false); 
    }






}
