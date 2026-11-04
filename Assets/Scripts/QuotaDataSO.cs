using UnityEngine;

[CreateAssetMenu(fileName = "QuotaDataSO", menuName = "Scriptable Objects/QuotaDataSO")]
public class QuotaDataSO : ScriptableObject
{
    [Header("Quota Completion Requisites")]
    [SerializeField] private float WeightMin;
    [SerializeField] private float WeightMax;
    [SerializeField] private int targetNum;
    [SerializeField] private float timeLimit;

    public float getWeightMin()
    {
        return WeightMin;
    }
    public float getWeightMax()
    {
        return WeightMax;
    }
    public int getTargetNum()
    {
        return targetNum;
    }
    public float getTimeLimit()
    {
        return timeLimit;
    }


}
