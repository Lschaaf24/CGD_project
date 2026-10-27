using System.Collections;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    private Transform objectToRespawn;
    [SerializeField] private GameObject respawnPos;
    
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
        objectToRespawn = other.GetComponent<Transform>();

        StartCoroutine(RespawnObject(objectToRespawn));
    }

    private IEnumerator RespawnObject(Transform go)
    {
        yield return new WaitForSeconds(1);

        go.gameObject.transform.position = respawnPos.transform.position;

        yield return null;

    }


}

