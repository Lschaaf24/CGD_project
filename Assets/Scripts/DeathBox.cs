using System;
using System.Collections;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    private Transform objectToRespawn;
    [SerializeField] private GameObject respawnPos;
    public event Action<Item> OnFishEntered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.TryGetComponent<BucketItem>(out _))
        {
            StartCoroutine(RespawnObject(other.transform));
            return;
        }

        if (other.TryGetComponent(out Item item) && item.GetItemData() is FishData)
        {
            OnFishEntered?.Invoke(item);
        }

        Destroy(other.gameObject);
    }

    

    private IEnumerator RespawnObject(Transform go)
    {
        yield return new WaitForSeconds(1);

        go.position = respawnPos.transform.position;
        if (go.GetComponent<Rigidbody>() != null)
        {
            go.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }


        yield return null;

    }


}

