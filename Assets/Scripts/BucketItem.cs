using Unity.VisualScripting;
using UnityEngine;

public class BucketItem : Item
{
    [SerializeField] GameObject puddle_task;
    protected override void ThrowItem(Vector3 direction, float throwSpeed) 
    {
        throwSpeedMult = Mathf.Lerp(minSpeedMult, maxSpeedMult, chargeRatio);

        throwSpeed = throwSpeed * throwSpeedMult;

        Instantiate(puddle_task, transform.position + (direction * throwSpeed * 2.0f), Quaternion.identity);

        Destroy(this.gameObject);
    }
}
