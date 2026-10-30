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
        if(collision.gameObject.GetComponent<Item>())
        {
            Debug.Log("score");
        }
        else
        {
            Debug.Log("No");
        }
    }
}
