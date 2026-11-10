using System;
using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] protected float maxChargeTime = 2.0f;

    [SerializeField] protected float minSpeedMult = 0.0f;
    [SerializeField] protected float maxSpeedMult = 1f;

    [SerializeField] private ItemData itemType;

    protected float throwSpeedMult;
    protected Rigidbody rb;
    private bool isCharging;
    private float charge = 0f;
    protected float chargeRatio;
    private float weight;
    private bool hasBeenWeighed = false;
    private bool canStun = false;

    private PlayerController thrower;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        weight = itemType.GenerateWeight();
    }

    protected virtual void Update()
    {
        if(transform.parent != null)
        {
            transform.localPosition = new Vector3(0, 1, 0);
        }

        if (isCharging)
        {
            charge = Mathf.Min(charge + Time.deltaTime, maxChargeTime);
            chargeRatio = Mathf.Clamp01(charge / maxChargeTime);

        }

        if (rb.linearVelocity.magnitude > 0.6f)
        {
            canStun = true;
        }
        else
        {
            canStun = false;
        }
    }

    public virtual void BeginCharge()
    {
        isCharging = true;
    }

    public virtual void ReleaseCharge(Vector3 direction, float throwHeight, float throwSpeed)
    {
        Debug.Log("throw");
        isCharging = false;
        Debug.Log("Direction: " + direction);
        ThrowItem(direction, throwHeight, throwSpeed);
    }

    protected virtual void ThrowItem(Vector3 direction, float throwHeight, float throwSpeed)
    {
        Debug.Log("Charge Ratio: " + chargeRatio);
        transform.SetParent(null);


        direction.y = throwHeight;

        Debug.Log("Direction: " + direction);

        throwSpeedMult = Mathf.Lerp(minSpeedMult, maxSpeedMult, chargeRatio);

        Debug.Log("ThrowSpeedMult: " + throwSpeedMult);

        float throwVelocity = throwSpeed * throwSpeedMult;
        Debug.Log("Throw Speed: " + throwVelocity);

        if(thrower != null)
        {
            Collider itemCollider = GetComponent<Collider>();
            Collider[] playerColliders = thrower.GetComponentsInChildren<Collider>();

            foreach (Collider playerCollider in playerColliders)
            {
                Physics.IgnoreCollision(itemCollider, playerCollider, true);
            }
        }

        rb.linearVelocity = new Vector3(0, 0, 0);
        rb.AddForce(direction * throwVelocity, ForceMode.Impulse);

        charge = 0;
        chargeRatio = 0;
         
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null && canStun)
            {
                playerController.stunPlayer();
            }
        }
    }

    public void SetThrower(PlayerController player)
    {
        thrower = player;
    }

    public void ResetThrowerCollision()
    {
        if (thrower != null)
        {
            Collider itemCollider = GetComponent<Collider>();
            Collider[] playerColliders = thrower.GetComponentsInChildren<Collider>();
            foreach (Collider playerCollider in playerColliders)
            {
                Physics.IgnoreCollision(itemCollider, playerCollider, false);
            }
        }
        thrower = null; 
    }

    public float GetChargeRatio()
    {
        return chargeRatio;
    }
    public Type GetItemType() 
    {
        return itemType.GetType();
    }

    public float GetWeight() 
    {
        return weight;
    }

    public bool GetHasBeenWeighed() 
    {
        return hasBeenWeighed;
    }

    public void SetHasBeenWeighed(bool weighed) 
    {
        hasBeenWeighed = weighed;
    }
    
}
