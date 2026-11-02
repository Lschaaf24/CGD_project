using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ObstacleMove : MonoBehaviour
{

    [SerializeField] private Transform startTrans;
    [SerializeField] private Transform endTrans;



    Vector3 direction;

    //[Range(-45, 45)]
    //[SerializeField] float angle = 0.0f;
    float angle = 0.0f;

    [Range(0, 10)]
    [SerializeField] float gravity = 0.0f;

    [SerializeField] float frictionCoefficient;

    float currentSpeed = 0.0f;

    [SerializeField] bool canFall = false;

    void Start()
    {
        direction = (endTrans.position - startTrans.position).normalized;
    }

    void Update()
    {
        float rawAngle = Camera.main.transform.eulerAngles.z;
        if (rawAngle > 180f) rawAngle -= 360f;
        angle = rawAngle;

        float rad = (angle) * Mathf.Deg2Rad;

        float gravityAcceleration = Mathf.Sin(rad) * gravity;

        float maxFrictionAcceleration = Mathf.Abs(Mathf.Cos(rad)) * gravity * frictionCoefficient;

        float netAcceleration = 0f;

        if (Mathf.Abs(currentSpeed) > 0.01f)
        {
            // If moving, friction pushes purely against the current direction of movement
            netAcceleration = gravityAcceleration - (Mathf.Sign(currentSpeed) * maxFrictionAcceleration);
        }
        else
        {
            // If standing still, gravity must overcome static friction to start moving
            if (Mathf.Abs(gravityAcceleration) > maxFrictionAcceleration)
            {
                // Object breaks free! It accelerates in the direction gravity is pulling
                netAcceleration = gravityAcceleration - (Mathf.Sign(gravityAcceleration) * maxFrictionAcceleration);
            }
            else
            {
                // Friction wins; the object stays completely still
                currentSpeed = 0f;
                netAcceleration = 0f;
            }
        }
        currentSpeed += netAcceleration * Time.deltaTime;

        transform.position += direction * currentSpeed * Time.deltaTime;

        if (!canFall) 
        {
            ClampToPath();
        }

    }

    private void ClampToPath()
    {
        float maxDistance = Vector3.Distance(startTrans.position, endTrans.position);

        float currentDistance = Vector3.Distance(startTrans.position, transform.position);

        if (currentSpeed < 0 && currentDistance > maxDistance || Vector3.Dot((transform.position - startTrans.position), (endTrans.position - startTrans.position)) < 0)
        {
            transform.position = startTrans.position;
            currentSpeed = 0f;
        }

        else if (currentSpeed > 0 && currentDistance >= maxDistance)
        {
            transform.position = endTrans.position;
            currentSpeed = 0f;
        }
    }
}
