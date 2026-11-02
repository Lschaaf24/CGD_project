using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private float itemWeight = 1f;
    virtual public float GenerateWeight()
    {
        return itemWeight;
    }
}
