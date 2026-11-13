using UnityEngine;


public class BillboardUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void LateUpdate() 
    {
        if (gameObject.activeInHierarchy) 
        {
            this.transform.LookAt(Camera.main.transform);
        }
    }
}
