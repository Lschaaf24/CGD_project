using System;
using UnityEngine;

public class WinCondition : MonoBehaviour
{

    public event Action OnEndGame;
    
    [Header("Refs")]
    [SerializeField] private LevelDataSO levelData;
    [SerializeField] private TimeChecker timeChecker;
    [SerializeField] private ScoreChecker scoreChecker;
    [Header("Trackers")]
    [SerializeField] private bool endGame = false;
    [SerializeField] private bool scoreReqReached = false;



    void Start()
    {
        levelData = GameManager.instance.GetLevelDataSO();
    }

   

    void Update()
    {

        /*if (endGame)
        {
            OnEndGame?.Invoke();
        }*/

    }

    

    void OnEnable()
    {
        timeChecker.OnTimeLimitReached += OnTimeReached;
        scoreChecker.OnScoreChanged += HandleScoreChanged;
    }

    private void HandleScoreChanged(int obj)
    {
        if(obj >= levelData.getScoreReq())
        {
            scoreReqReached = true;
        }
        else
        {
            scoreReqReached = false;
        }
    }

    private void OnTimeReached()
    {
        OnEndGame?.Invoke();
    }

    void OnDisable()
    {
        timeChecker.OnTimeLimitReached -= OnTimeReached;
        scoreChecker.OnScoreChanged -= HandleScoreChanged;

    }

    public bool GetScoreReqReached()
    {
        return scoreReqReached;
    }
}
