using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private LevelSelector currenLevel;
    private States currentState;

    UnityEvent SwitchedStates;


    enum States
    {
        LevelSelectionScreen,
        GamePlay,
        Pause,
    }




    void Start()
    {
        if (SwitchedStates == null)
        { 
            SwitchedStates = new UnityEvent();
        }

        SwitchedStates.AddListener(OnEventTriggered);
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
