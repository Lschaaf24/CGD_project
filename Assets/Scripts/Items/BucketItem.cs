using Unity.VisualScripting;
using UnityEngine;

public class BucketItem : Item
{
    [SerializeField] GameObject puddle_task;
    [SerializeField] GameObject water;

    bool isFull = false;
    protected override void ThrowItem(Vector3 direction, float throwHeight, float throwSpeed) 
    {
        Debug.Log("THROW");
        if (!isFull)
        {
            base.ThrowItem(direction, throwHeight, throwSpeed);
            return;
        }

        throwSpeedMult = Mathf.Lerp(minSpeedMult, maxSpeedMult, chargeRatio);

        throwSpeed = throwSpeed * throwSpeedMult;

        Vector3 position = transform.position;
        position.y = 0.01f;
        direction.y = 0;


        RaycastHit hit;
        if (Physics.Raycast(position + (direction * throwSpeed), -Vector3.up, out hit, 10f, 1 << LayerMask.NameToLayer("Ground")))
        {
            Instantiate(puddle_task, new Vector3(position.x + (direction * throwSpeed).x, hit.point.y, position.z + (direction * throwSpeed).z), Quaternion.identity);
        }

        SetFill(false);
    }

    public void SetFill(bool fill) 
    {
        isFull = fill;
        if (isFull)
        {
            water.GetComponent<MeshRenderer>().material.color = Color.blue;    
        }
        else 
        {
            water.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }

    public bool GetIsFull() 
    {
        return isFull;
    }
}
