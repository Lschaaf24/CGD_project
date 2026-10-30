using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class Item : MonoBehaviour
{
    [SerializeField] protected float maxChargeTime = 2.0f;

    [SerializeField] protected float minSpeedMult = 0.0f;
    [SerializeField] protected float maxSpeedMult = 1f;

    protected float throwSpeedMult;

    private Rigidbody rb;

    private bool isCharging;
    private float charge = 0f;
    protected float chargeRatio;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
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
}
