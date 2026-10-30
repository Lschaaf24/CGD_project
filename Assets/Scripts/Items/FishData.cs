using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "Scriptable Objects/FishData")]
public class FishData : ItemData
{
    [SerializeField] private float minWeight = 1.0f;
    [SerializeField] private float maxWeight = 12.0f;

    public float GenerateRandomWeight()
    {
        return Random.Range(minWeight, maxWeight);
    }
}
