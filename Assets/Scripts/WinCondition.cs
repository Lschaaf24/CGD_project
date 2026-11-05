using System;
using UnityEngine;

public class WinCondition : MonoBehaviour
{

    public event Action OnEndGame;
    [Header("Refs")]
    [SerializeField] private LevelDataSO levelData;
    [SerializeField] private TimeChecker timeChecker;
    [Header("Trackers")]
    [SerializeField] private bool endGame = false;




    void Start()
    {
        levelData = GameManager.instance.GetLevelDataSO();
    }

   

    void Update()
    {
        if (endGame)
        {
            OnEndGame?.Invoke();
        }

    }

    

    void OnEnable()
    {
        timeChecker.OnTimeLimitReached += OnTimeReached;
    }

    private void OnTimeReached()
    {
        OnEndGame?.Invoke();
    }

    void OnDisable()
    {
        timeChecker.OnTimeLimitReached -= OnTimeReached;
    }


}
