using UnityEngine;

public class BasketTask : MonoBehaviour
{

  

    void Awake()
    {


    }


    void Update()
    {
       

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Item>() == null) return;
        if(other.gameObject.GetComponent<Item>().GetItemType() == typeof(FishData))
        {
            Debug.Log("score");
            Destroy(other.gameObject);
        }
        else
        {
            Debug.Log("No");
        }
    }

   
}
