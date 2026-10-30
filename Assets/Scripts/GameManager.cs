using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] private LevelSelector levelSelector;
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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        if (SwitchedStates == null)
        { 
            SwitchedStates = new UnityEvent();
        }

        SwitchedStates.AddListener(OnEventTriggered);


       /* levelselector = GetComponent<LevelSelector>();
        currentLevel = levelselector.GetCurrentLevel();*/


    }

    private void OnEventTriggered()
    {

        Debug.Log("States Switched");

    }

    void Update()
    {
        if (levelSelector == null)
        {
            TryGetLevelSelector();
        }
        /*  if( SwitchedStates != null)
          {
              SwitchedStates.Invoke();
          }*/

    }

    private void TryGetLevelSelector()
    {
        if (LevelSelector.Instance != null)
        {
            levelSelector = LevelSelector.Instance;
            currentLevel = levelSelector.GetCurrentLevel();

            Debug.Log("LevelSelector successfully assigned!");
        }
    }

    private LevelSelector LevelSelector
    {
        get
        {
            if (levelSelector == null)
                levelSelector = LevelSelector.Instance;

            return levelSelector;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
