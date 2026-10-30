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

    }

    private void OnLevelFinished()
    {
        Debug.Log("level finish Reached");
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
