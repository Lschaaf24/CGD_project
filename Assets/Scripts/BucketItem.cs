using Unity.VisualScripting;
using UnityEngine;

public class BucketItem : Item
{
    [SerializeField] GameObject puddle_task;
    protected override void ThrowItem(Vector3 direction, float throwSpeed) 
    {
        throwSpeedMult = Mathf.Lerp(minSpeedMult, maxSpeedMult, chargeRatio);

        throwSpeed = throwSpeed * throwSpeedMult;

        Vector3 position = transform.position;
        position.y = 0.01f;
        direction.y = 0;

        if (Physics.Raycast(position + (direction * throwSpeed), -Vector3.up, 10f, 1 << LayerMask.NameToLayer("Ground")))
        {
            Instantiate(puddle_task, position + (direction * throwSpeed), Quaternion.identity);
        }


        Destroy(this.gameObject);
    }
}
