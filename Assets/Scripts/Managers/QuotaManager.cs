using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [Header("Refs")]
    [SerializeField] private BasketTask basket;
    [SerializeField] private DeathBox deathbox;
    [SerializeField] private CanvasGroup captainUI;
    [SerializeField] private TMP_Text captainWeightTxt;
    [SerializeField] private TMP_Text captainProgressTxt;

    [Header("Quota Settings")]
    [SerializeField] private List<QuotaDataSO> availableQuotas;
    [SerializeField] private Quota quotaPrefab;
    [SerializeField] private int maxQuotas = 5;

    [Header("Captain Speech")]
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float holdTime = 3f;
    [SerializeField] private float fadeOutTime = 0.5f;

    private bool captainBusy;

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
        captainUI.alpha = 0f;
    }
    public bool TryRandomGenerateQuota()
    {
        if (!acceptingQuotas || captainBusy) return false;
        if (activeQuotas.Count >= maxQuotas) return false;

        var data = availableQuotas[Random.Range(0, availableQuotas.Count)];
        StartCoroutine(AnnounceThenAddQuota(data));
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
        deathbox.OnCargoEntered += HandleCargoReleased;
    }

    void OnDisable()
    {
        basket.OnFishCollected -= HandleFishCollected;
        deathbox.OnFishEntered -= HandleFishRelease;
        deathbox.OnCargoEntered -= HandleCargoReleased;
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
    private void HandleCargoReleased()
    {
        OnQuotaSDGFailed?.Invoke();
    }
    private IEnumerator AnnounceThenAddQuota(QuotaDataSO data)
    {
        captainBusy = true;

        captainWeightTxt.text = $"{data.getWeightMin()} - {data.getWeightMax()}";
        captainProgressTxt.text = $"{data.getTargetNum()}x";

        yield return FadeCaptain(0f, 1f, fadeInTime);
        yield return new WaitForSeconds(holdTime);
        yield return FadeCaptain(1f, 0f, fadeOutTime);

        captainBusy = false;

        if (!acceptingQuotas) yield break;

        AddQuota(data);
    }

    private IEnumerator FadeCaptain(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            captainUI.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        captainUI.alpha = to;
    }

    private void AddQuota(QuotaDataSO data)
    {
        var quota = Instantiate(quotaPrefab);
        quota.Init(data);

        quota.OnCompleted += HandleCompleted;
        quota.OnFailed += HandleFailed;
        quota.OnSDGPassed += HandleSDGPass;

        activeQuotas.Add(quota);
        OnQuotaAdded?.Invoke(quota);
    }

    public void ClearAll()
    {
        acceptingQuotas = false;
        StopAllCoroutines();
        captainBusy = false;
        captainUI.alpha = 0f;

        for (int i = activeQuotas.Count - 1; i >= 0; i--)
            Remove(activeQuotas[i]);
    }


}
