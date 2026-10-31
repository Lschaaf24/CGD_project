using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    UnityEvent QuotaReached;
    UnityEvent TimeLimitReached;
    UnityEvent EndGame;

    [SerializeField] private bool endGame = false;
    [SerializeField] private LevelDataSO levelData;

    void Start()
    {

        if (QuotaReached == null)
        {
            QuotaReached = new UnityEvent();
        }

        QuotaReached.AddListener(OnQuotaReached);

        if (TimeLimitReached == null)
        {
            TimeLimitReached = new UnityEvent();
        }

        TimeLimitReached.AddListener(OnTimeLimitReached);

        if (EndGame == null)
        {
            EndGame = new UnityEvent();
        }

        EndGame.AddListener(OnLevelFinished);

        levelData = GameManager.instance.GetLevelDataSO();
    }

    private void OnLevelFinished()
    {
        Debug.Log("level finish Reached");

        LevelSelector.Instance.AddLevelToCompletedList(levelData);
        LevelSelector.Instance.SetCurrentLevelData(null);


        
        SceneManager.LoadScene("LevelSelector");
    }

    private void OnTimeLimitReached()
    {
        Debug.Log("Time Limit Reached");
    }

    private void OnQuotaReached()
    {
        Debug.Log("Quota Reached");

    }

    void Update()
    {
        if (endGame == true && EndGame != null)
        {
            OnLevelFinished();
        }
    }
}
