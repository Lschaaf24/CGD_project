using UnityEngine;
using UnityEngine.Events;

public class Task : MonoBehaviour
{
   
   
    [Header("Task Stats")]


    [Tooltip("How long the player has to complete the task in seconds")]
    [Range(1, 20)]
    [SerializeField] private float time_limit = 1.0f;

    [Tooltip("The score the player will achieve from completing the task")]
    [SerializeField] private uint score_addition = 0;


    [Tooltip("The number of players required to complete the task")]
    [Range(1, 4)]
    [SerializeField] private int players_required = 1;

    [Tooltip("Will the task always persist or does it spawn temporarily")]
    [SerializeField] private bool persistent_task = false;

    [Tooltip("Does the task require continous input?")]
    [SerializeField] private bool continuous_task = false;

    [Space(15)]

    [Header("FX")]

    [InspectorName("Animation")]
    [Tooltip("Animation to be played on the player when interacting with the task")]
    [SerializeField] private Animation animation;

    [InspectorName("Sound Effect")]
    [Tooltip("Sound Effect to be played when the player completes the task")]
    [SerializeField] private AudioClip sound_effect;

    [InspectorName("Particle System")]
    [Tooltip("Particle Effect to be played when the task is ready")]
    [SerializeField] private ParticleSystem particle_system;

    bool is_completed = false;
    int current_players = 0;
    bool in_task = false;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (in_task) 
        {

        }   
    }

    public bool EnterTask() 
    {
        Debug.Log("ENTERED");
        if(current_players < players_required) 
        {
            current_players++;
            if(current_players == players_required) 
            {
                BeginTask();
            }
            return true;
        }
        return false;
    }


    public void BeginTask() 
    {
        if (continuous_task) 
        {

        }
        EndTask();
    }

    public void EndTask()
    {
        Debug.Log("TASK COMPLETED");
        if (persistent_task) 
        {
            is_completed = true;
            current_players = 0;
            return;
        }
        Destroy(this.gameObject);

        
    }

     




}
