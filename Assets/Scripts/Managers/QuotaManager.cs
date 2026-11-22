using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuotaManager : MonoBehaviour
{
    public static QuotaManager instance;

    public event Action<QuotaDataSO> OnQuotaCompleted;
    public event Action<QuotaDataSO> OnQuotaFailed;
    public event Action<Quota> OnQuotaAdded;
    public event Action<Quota> OnQuotaRemoved;
    public event Action OnQuotaSDGFailed;
    public event Action OnQuotaSDGPassed;

    [SerializeField] private BasketTask basket;
    [SerializeField] private DeathBox deathbox;

    [SerializeField] private List<QuotaDataSO> availableQuotas;
    [SerializeField] private Quota quotaPrefab;
    [SerializeField] private int maxQuotas = 5;

    private readonly List<Quota> activeQuotas = new();
    private bool acceptingQuotas = true;

    public int ActiveCount => activeQuotas.Count;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        maxQuotas = GameManager.instance.GetLevelDataSO().getQuotaLimit();
    }
    public bool TryRandomGenerateQuota()
    {
        if (!acceptingQuotas) return false;
        if (activeQuotas.Count >= maxQuotas) return false;

        var data = availableQuotas[Random.Range(0,availableQuotas.Count)];
        var quota = Instantiate(quotaPrefab);
        quota.Init(data);

        quota.OnCompleted += HandleCompleted;
        quota.OnFailed += HandleFailed;
        quota.OnSDGPassed += HandleSDGPass;

        activeQuotas.Add(quota);
        OnQuotaAdded?.Invoke(quota);
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

    private void HandleSDGPass()
    {
        OnQuotaSDGPassed?.Invoke();
    }

    private void Remove(Quota quota)
    {
        quota.OnCompleted -= HandleCompleted;
        quota.OnFailed -= HandleFailed;
        quota.OnSDGPassed -= HandleSDGPass;
        activeQuotas.Remove(quota);
        OnQuotaRemoved?.Invoke(quota);
        Destroy(quota.gameObject);
    }

    public void ClearAll()
    {
        acceptingQuotas = false;
        for(int i = activeQuotas.Count - 1; i >= 0;i--)
        {
            Remove(activeQuotas[i]);
        }
    }

    public bool MatchesActiveQuotas(Item item)
    {
        foreach(var quota in activeQuotas.ToArray())
        {
            if(quota.Accepts(item)) return true;
        }
        return false;
    }

    void OnEnable()
    {
        basket.OnFishCollected += HandleFishCollected;
        deathbox.OnFishEntered += HandleFishRelease;
    }


    void OnDisable()
    {
        basket.OnFishCollected -= HandleFishCollected;
        deathbox.OnFishEntered -= HandleFishRelease;
    }
    public void HandleFishCollected(Item item)
    {
        bool accepted = false;
        foreach (var quota in activeQuotas.ToArray())
        {
            if (quota.TryAddProgress(item)) { accepted = true; break; }
        }

        if (!accepted) OnQuotaSDGFailed?.Invoke();
    }

    private void HandleFishRelease(Item fish)
    {
        if (MatchesActiveQuotas(fish)) OnQuotaSDGFailed?.Invoke();
        else OnQuotaSDGPassed?.Invoke();
    }
}
