using Unity.VisualScripting;
using UnityEngine;

public class WaterProjectileCollision : MonoBehaviour
{
    [SerializeField] GameObject puddleTask;
    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(puddleTask, collision.GetContact(0).point, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
