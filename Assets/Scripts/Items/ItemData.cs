using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private float itemWeight = 1f;
    [SerializeField] private Mesh itemMesh;
    public string itemName;

    virtual public float GenerateWeight()
    {
        return itemWeight;
    }

    virtual public Mesh GetItemMesh() 
    {
        return (itemMesh);
    }
}
