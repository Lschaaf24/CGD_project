using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuotaManager : MonoBehaviour
{
    public event Action<QuotaDataSO> OnQuotaCompleted;
    public event Action<QuotaDataSO> OnQuotaFailed;

    [SerializeField] private List<QuotaDataSO> availableQuotas;
    [SerializeField] private Quota quotaPrefab;
    [SerializeField] private int maxQuotas = 5;

    private readonly List<Quota> activeQuotas = new();

    public int ActiveCount => activeQuotas.Count;


    public bool RandomGenerateQuota()
    {
        if (activeQuotas.Count >= maxQuotas) return false;

        var data = availableQuotas[Random.Range(0,availableQuotas.Count)];
        var quota = Instantiate(quotaPrefab);
        quota.Init(data);

        quota.OnCompleted += HandleCompleted;
        quota.OnFailed += HandleFailed;

        activeQuotas.Add(quota);
        Debug.Log("HERE");
        return true;
    }


    private void HandleCompleted(Quota quota)
    {
        Remove(quota);
        OnQuotaCompleted?.Invoke(quota.Data);
    }
    private void HandleFailed(Quota quota)
    {
        Remove(quota);
        OnQuotaFailed?.Invoke(quota.Data);
    }

    private void Remove(Quota quota)
    {
        quota.OnCompleted -= HandleCompleted;
        quota.OnFailed -= HandleFailed;
        activeQuotas.Remove(quota);
        Destroy(quota.gameObject);
    }

    public void ClearAll()
    {
        for(int i = activeQuotas.Count - 1; i >= 0;)
        {
            Remove(activeQuotas[i]);
        }
    }


}
