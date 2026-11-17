using TMPro;
using UnityEngine;

public class QuotaUI : MonoBehaviour
{
    [SerializeField] private TMP_Text TitleText;
    [SerializeField] private TMP_Text ProgressText;
    [SerializeField] private TMP_Text TimeLeftText;

    private Quota quota;


    public void StartUI(Quota q)
    {
        quota = q;
        quota.OnProgressChanged += HandleProgress;

        TitleText.text = $"{quota.Data.getWeightMin()} - {quota.Data.getWeightMax()}";
        HandleProgress(0, q.Data.getTargetNum());
    }

    public void RemoveUI()
    {
        if(quota != null) quota.OnProgressChanged -= HandleProgress;
        quota = null;
    }

    private void HandleProgress(int progress, int target)
    {
        ProgressText.text = $"{progress}/{target}";
    }

    private void Update()
    {
        if (quota != null)
        {
            TimeLeftText.text = quota.TimeLeft.ToString(".00");
        }
    }

    private void OnDestroy()
    {
        RemoveUI();
    }

}
