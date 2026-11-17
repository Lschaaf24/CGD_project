using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class BucketItem : Item
{
    [SerializeField] GameObject waterProjectile;
    [SerializeField] GameObject water;
    [SerializeField] private ParticleSystem throwWater;

    bool isFull = false;
    protected override void ThrowItem(Vector3 direction, float throwHeight, float throwSpeed) 
    {
        Debug.Log("THROW");
        if (!isFull)
        {
            base.ThrowItem(direction, throwHeight, throwSpeed);
            return;
        }

        direction.y = throwHeight;

        throwSpeedMult = Mathf.Lerp(minSpeedMult, maxSpeedMult, chargeRatio);

        //throwSpeed = throwSpeed * throwSpeedMult;
        float throwVelocity = throwSpeed * throwSpeedMult;

        var proj = Instantiate(waterProjectile, transform.position + Vector3.up, Quaternion.identity);

        if (thrower != null)
        Instantiate(throwWater,this.transform.position, Quaternion.LookRotation(direction));

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position + (direction * throwSpeed), -Vector3.up, out hit, 10f, 1 << LayerMask.NameToLayer("Ground")))
        {
            Collider itemCollider = proj.GetComponent<Collider>();
            Collider[] playerColliders = thrower.GetComponentsInChildren<Collider>();

            foreach (Collider playerCollider in playerColliders)
            {
                Physics.IgnoreCollision(itemCollider, playerCollider, true);
            }
        }

        rb.linearVelocity = new Vector3(0, 0, 0);
        proj.GetComponent<Rigidbody>().AddForce(direction * throwVelocity, ForceMode.Impulse);

   
        SetFill(false);
    }



    public void SetFill(bool fill) 
    {
        isFull = fill;
        if (isFull)
        {
            water.GetComponent<MeshRenderer>().enabled = true;  
        }
        else 
        {
            water.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public bool GetIsFull() 
    {
        return isFull;
    }
}
