using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] protected int itemWeight = 1;
    [SerializeField] private Mesh itemMesh;
    public string itemName;

    virtual public int GenerateWeight()
    {
        return itemWeight;
    }

    virtual public int GetWeight() { return itemWeight; }

    virtual public Mesh GetItemMesh() 
    {
        return (itemMesh);
    }
}
