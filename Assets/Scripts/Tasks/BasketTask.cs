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
        //Instantiate(basketText, transform.position, Quaternion.identity);
       
        basketText.text = weight.ToString();
        if(weight > 5f)
        {
            basketText.color = Color.green;
        }
        else
        {
            basketText.color = Color.red;
        }
        
        basketText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        basketText.gameObject.SetActive(false);

    }
   
}
