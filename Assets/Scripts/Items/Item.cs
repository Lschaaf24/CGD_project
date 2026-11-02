using System;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    }

    public virtual void BeginCharge()
    {
        isCharging = true;
    }

    public virtual void ReleaseCharge(Vector3 direction, float throwSpeed)
    {
        Debug.Log("throw");
        isCharging = false;
        ThrowItem(direction, throwSpeed);

        GetComponent<Collider>().enabled = true;
    }

    protected virtual void ThrowItem(Vector3 direction, float throwSpeed)
    {
        transform.SetParent(null);

        if(transform.parent == null)
        {
            Debug.Log(chargeRatio);
            throwSpeedMult = Mathf.Lerp(minSpeedMult, maxSpeedMult, chargeRatio);
            Debug.Log("throw speed mult" + throwSpeedMult);
            float throwVelocity = throwSpeed * throwSpeedMult;

            Debug.Log("Throw Speed: " + throwVelocity);
            rb.AddForce(direction * throwVelocity, ForceMode.Impulse);

            charge = 0;

        }
    }

    public Type GetItemType() 
    {
        return itemType.GetType();
    }

    public float GetWeight() 
    {
        return weight;
    }
}
