using System;
using Unity.VisualScripting;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] private LevelSelector levelSelector;
    private States currentState;
    [SerializeField] private LevelDataSO currentLevel;
    [SerializeField] private WinCondition winChecker;


    enum States
    {
        GamePlay,
        Pause,
    }

    private void Awake()
    {
        instance = this;
        TryGetLevelSelector();
    }


    void Start()
    {

    }



    void Update()
    {
        

    }

    private void TryGetLevelSelector()
    {
        if (LevelSelector.Instance != null)
        {
            levelSelector = LevelSelector.Instance;
            currentLevel = levelSelector.GetCurrentLevelData();

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
   
    public LevelDataSO GetLevelDataSO()
    {
        return currentLevel;
    }

    private void SetState(States newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case States.Pause:
                Time.timeScale = 0f;
                break;
            case States.GamePlay:
                Time.timeScale = 1f;
                break;
        }
    }

    void OnEnable()
    {
        winChecker.OnEndGame += OnLevelFinished;
    }

    void OnDisable()
    {
        winChecker.OnEndGame -= OnLevelFinished;
    }

    private void OnLevelFinished()
    {
        Debug.Log("level finish Reached");
        levelSelector.AddLevelToCompletedList(currentLevel);



        SceneManager.LoadScene("LevelSelector");
        levelSelector.SetCurrentLevelData(null);
    }
}
