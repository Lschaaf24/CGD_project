using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SplashEffect : MonoBehaviour
{
    [SerializeField] GameObject particleSystem;
    public Queue<GameObject> effectPool = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < 10; i++) 
        {
            GameObject effect = Instantiate(particleSystem, Vector3.up * 200, Quaternion.identity);
            effect.transform.SetParent(transform, true);
            effectPool.Enqueue(effect);
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        GameObject effect = effectPool.Dequeue();
        effect.transform.position = collision.transform.position;
        effect.GetComponent<ParticleSystem>().Play();
        effectPool.Enqueue(effect);

    }
}
