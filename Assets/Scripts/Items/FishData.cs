using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "Scriptable Objects/FishData")]
public class FishData : ItemData
{
    [SerializeField] private int minWeight = 1;
    [SerializeField] private int maxWeight = 12;
    [SerializeField] private bool isFish = true;


    override public int GenerateWeight()
    {
        itemWeight = Random.Range(minWeight, maxWeight);
        return itemWeight;
    }

    public override int GetWeight()
    {
        return base.GetWeight();
    }





}
