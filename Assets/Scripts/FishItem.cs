using System.ComponentModel;
using UnityEngine;

public class FishItem : Item
{
     [SerializeField] float weight = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weight = Random.Range(0.1f, 12.0f);
    }
}
