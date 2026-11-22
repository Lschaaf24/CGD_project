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
    [SerializeField] private float sustainableMult = 1.0f;
    [SerializeField] private float sustainableMultIncrease = 0.3f;
    [SerializeField] private float sustainableMultDecrease = 0.5f;
    [SerializeField] private int sustainableScore = 0;

    private void Start()
    {
        scoreText.text = $"Score: 0";
    }
    private void OnEnable()
    {
        quotaManager.OnQuotaCompleted += HandleScoreAdded;
        quotaManager.OnQuotaFailed += HandleScoreSubtracted;
        quotaManager.OnQuotaSDGPassed += HandleSDGScoreAdded;
        quotaManager.OnQuotaSDGFailed += HandleSDGScoreSubtracted;
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
        quotaManager.OnQuotaSDGPassed -= HandleSDGScoreAdded;
        quotaManager.OnQuotaSDGFailed -= HandleSDGScoreSubtracted;
    }

    private void HandleSDGScoreSubtracted()
    {
        sustainableMult -= sustainableMultDecrease;
    }

    private void HandleSDGScoreAdded()
    {
        sustainableMult += sustainableMultIncrease;
    }

    public int GetScoreFromScoreChecker()
    {
        return score;
    }

    public void AddSustainableScore()
    {
        sustainableScore = (int)(score * sustainableMult);
        score += sustainableScore;
        OnScoreChanged(score);
    }
}
