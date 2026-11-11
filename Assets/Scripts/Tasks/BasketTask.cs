using System;
using UnityEngine;
using static UnityEditor.Progress;
using TMPro;
using System.Collections;

public class BasketTask : MonoBehaviour
{

    public event Action<Item> OnFishCollected;
    [SerializeField] private ParticleSystem basketParticle;
    [SerializeField] private TMP_Text basketText;
    [SerializeField] private Canvas canvas;

    private int weight;



    void Awake()
    {


    }


    void Update()
    {
       

    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Item item)) return;

        if(item.GetItemType() == typeof(FishData))
        {
            Debug.Log("score");
            OnFishCollected?.Invoke(item);
            Destroy(other.gameObject);
            basketParticle.Play();
            weight = (int)other.GetComponent<Item>().GetWeight();
            StartCoroutine(ShowText());
        }
        else
        {
            Debug.Log("No");
        }
    }


    private IEnumerator ShowText()
    {
       
       
        basketText.text = weight.ToString();
        TMP_Text text = Instantiate(basketText, canvas.transform);

        if(weight > 5f)
        {
            text.color = Color.green;
        }
        else
        {
            text.color = Color.red;
        }
        
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        Destroy(text.gameObject);
       


    }
   
}
