using System;
using UnityEngine;
using static UnityEditor.Progress;

public class BasketTask : MonoBehaviour
{

    public event Action<Item> OnFishCollected;

    void Awake()
    {


    }


    void Update()
    {
       

    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Item item)) return;

        if(item.GetItemData().GetType() == typeof(FishData))
        {
            Debug.Log("score");
            OnFishCollected?.Invoke(item);
            Destroy(other.gameObject);
        }
        else
        {
            Debug.Log("No");
        }
    }

   
}
