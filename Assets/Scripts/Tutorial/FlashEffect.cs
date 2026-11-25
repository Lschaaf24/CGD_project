using Unity.VisualScripting;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    [SerializeField] Renderer render;
    [SerializeField] float duration = 0.15f;
    [SerializeField] Material flashmaterial;


    Material originalMaterial;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
