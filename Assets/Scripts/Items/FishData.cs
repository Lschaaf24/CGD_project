using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "Scriptable Objects/FishData")]
public class FishData : ItemData
{
    [SerializeField] private float minWeight = 1.0f;
    [SerializeField] private float maxWeight = 12.0f;
    [SerializeField] private bool isFish = true;


    override public float GenerateWeight()
    {
        itemWeight = Random.Range(minWeight, maxWeight);
        return itemWeight;
    }

    public override float GetWeight()
    {
        return base.GetWeight();
    }





}
