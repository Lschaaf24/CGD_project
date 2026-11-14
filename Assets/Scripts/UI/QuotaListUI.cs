using System.Collections.Generic;
using UnityEngine;

public class QuotaListUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private QuotaManager quotaManager;
    [SerializeField] private Transform listParent;
    [SerializeField] private QuotaUI quotaUI;

    private readonly Dictionary<Quota, QuotaUI> Quotas = new();

    private void OnEnable()
    {
        quotaManager.OnQuotaAdded += HandleAdded;
        quotaManager.OnQuotaRemoved += HandleRemoved;
    }
    private void OnDisable()
    {
        quotaManager.OnQuotaAdded -= HandleAdded;
        quotaManager.OnQuotaRemoved -= HandleRemoved;
    }
    private void HandleAdded(Quota quota)
    {
        var UI = Instantiate(quotaUI, listParent);
        UI.StartUI(quota);
        Quotas.Add(quota,UI);
    }

    private void HandleRemoved(Quota quota)
    {
        if (!Quotas.TryGetValue(quota, out var UI)) return;

        UI.RemoveUI();
        Destroy(UI.gameObject);
        Quotas.Remove(quota);
    }

}
