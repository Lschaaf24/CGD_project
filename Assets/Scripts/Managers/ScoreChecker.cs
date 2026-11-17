using System;
using TMPro;
using UnityEngine;

public class ScoreChecker : MonoBehaviour
{
    public event Action<int> OnScoreChanged;


    [Header("refs")]
    [SerializeField] private QuotaManager quotaManager;
    [SerializeField] private TMP_Text scoreText;
    [Header("Score settings")]
    [SerializeField] private int score;

    private void Start()
    {
        scoreText.text = $"Score: 0";
    }
    private void OnEnable()
    {
        quotaManager.OnQuotaCompleted += HandleScoreAdded;
        quotaManager.OnQuotaFailed += HandleScoreSubtracted;
    }

    private void HandleScoreSubtracted(QuotaDataSO obj)
    {
        score = obj.getScore();
        scoreText.text = $"Score: {score.ToString()}";
        OnScoreChanged(score);
    }

    private void HandleScoreAdded(QuotaDataSO obj)
    {
        score += obj.getScore();
        scoreText.text = $"Score: {score.ToString()}";
        OnScoreChanged(score);
    }

    private void OnDisable()
    {
        quotaManager.OnQuotaCompleted -= HandleScoreAdded;
        quotaManager.OnQuotaFailed -= HandleScoreSubtracted;
    }

    public int GetScoreFromScoreChecker()
    {
        return score;
    }

}
