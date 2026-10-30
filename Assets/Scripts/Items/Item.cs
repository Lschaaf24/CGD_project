using System;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;


public class Item : MonoBehaviour
{
    [SerializeField] protected float maxChargeTime = 1.2f;

    [SerializeField] protected float minSpeedMult = 0.5f;
    [SerializeField] protected float maxSpeedMult = 1f;
    [SerializeField] protected float throwSpeed = 15f;

    [SerializeField] private ItemData itemType;

    protected float throwSpeedMult;

    protected Rigidbody rb;

    private bool isCharging;
    private float charge;
    protected float chargeRatio;

    private float weight;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (itemType is FishData fishData)
        {
            weight = fishData.GenerateRandomWeight();
        }
    }

    protected virtual void Update()
    {
        if(transform.parent != null)
        {
            transform.localPosition = new Vector3(1, 0, 0);

        }

        if (isCharging)
        {
            Debug.Log("charging");
            charge = Mathf.Min(charge + Time.fixedDeltaTime, maxChargeTime);
            chargeRatio = Mathf.Clamp01(charge / maxChargeTime);
        }
    }

    public virtual void BeginCharge()
    {

        isCharging = true;
        charge = 0;

    }

    public virtual void ReleaseCharge(Vector3 direction)
    {
        Debug.Log("throw");
        isCharging = false;
        ThrowItem(direction);
        charge = 0;
    }

    protected virtual void ThrowItem(Vector3 direction)
    {
        transform.SetParent(null);
        throwSpeedMult = Mathf.Lerp(minSpeedMult, maxSpeedMult, chargeRatio);

        throwSpeed = throwSpeed * throwSpeedMult;


        rb.AddForce(direction * throwSpeed * Time.fixedDeltaTime);
    }

    public Type GetItemType() 
    {
        return itemType.GetType();
    }
}
