using UnityEngine;

public class BasketTask : MonoBehaviour
{

  

    void Awake()
    {


    }


    void Update()
    {
       

    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Item>().GetItemType() == typeof(FishData))
        {
            Debug.Log("score");
            Destroy(collision.gameObject);
        }
        else
        {
            Debug.Log("No");
        }
    }
}
