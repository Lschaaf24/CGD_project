using System;
using UnityEngine;

public class ScoreChecker : MonoBehaviour
{
    public event Action<int> OnScoreChanged;


    [Header("refs")]
    [SerializeField] private QuotaManager quotaManager;
    [Header("Score settings")]
    [SerializeField] private int score;

    private void Start()
    {

    }
    private void OnEnable()
    {
        quotaManager.OnQuotaCompleted += HandleScoreAdded;
        quotaManager.OnQuotaFailed += HandleScoreSubtracted;
    }

    private void HandleScoreSubtracted(QuotaDataSO obj)
    {
        score = obj.getScore();
        OnScoreChanged(score);
    }

    private void HandleScoreAdded(QuotaDataSO obj)
    {
        score += obj.getScore();
        OnScoreChanged(score);
    }

    private void OnDisable()
    {
        quotaManager.OnQuotaCompleted -= HandleScoreAdded;
        quotaManager.OnQuotaFailed -= HandleScoreSubtracted;
    }

}
