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
    [SerializeField] private int playersRequired = 1;

    [Tooltip("Will the task always persist or does it spawn temporarily")]
    [SerializeField] private bool persistentTask = false;

    [Tooltip("Does the task require continous input?")]
    [SerializeField] private bool continuousTask = false;

    [Space(15)]

    [Header("FX")]

    [InspectorName("Animation")]
    [Tooltip("Animation to be played on the player when interacting with the task")]
    [SerializeField] private Animation animation;

    [InspectorName("Sound Effect")]
    [Tooltip("Sound Effect to be played when the player completes the task")]
    [SerializeField] private AudioClip soundEffect;

    [InspectorName("Particle System")]
    [Tooltip("Particle Effect to be played when the task is ready")]
    [SerializeField] private ParticleSystem particlSystem;

    bool isCompleted = false;
    int currentPlayers = 0;
    bool taskStarted = false;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (taskStarted) 
        {

        }   
    }

    public bool EnterTask() 
    {
        Debug.Log("ENTERED");
        if(currentPlayers < playersRequired) 
        {
            currentPlayers++;
            if(currentPlayers == playersRequired) 
            {
                BeginTask();
            }
            return true;
        }
        return false;
    }


    public void BeginTask() 
    {
        if (continuousTask) 
        {

        }
        EndTask();
    }

    public void EndTask()
    {
        Debug.Log("TASK COMPLETED");
        if (persistentTask) 
        {
            isCompleted = true;
            currentPlayers = 0;
            return;
        }
        Destroy(this.gameObject);

        
    }

     




}
