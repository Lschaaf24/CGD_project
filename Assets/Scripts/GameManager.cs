using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] private LevelSelector LevelSelector;
    private States currentState;
    [SerializeField] private Level currentLevel;


    UnityEvent SwitchedStates;


    enum States
    {
        GamePlay,
        Pause,
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }


    void Start()
    {
        if (SwitchedStates == null)
        { 
            SwitchedStates = new UnityEvent();
        }

        SwitchedStates.AddListener(OnEventTriggered);

        LevelSelector = GetComponent<LevelSelector>();
        currentLevel = LevelSelector.GetCurrentLevel();


    }

    private void OnEventTriggered()
    {

        Debug.Log("States Switched");

    }

    void Update()
    {
      /*  if( SwitchedStates != null)
        {
            SwitchedStates.Invoke();
        }*/

    }
}
