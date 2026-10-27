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

    [Range(-45, 45)]
    [SerializeField] float angle = 0.0f;

    [Range(0, 10)]
    [SerializeField] float gravity = 0.0f;

    float currentSpeed = 0.0f;

    [SerializeField] bool canFall = false;

    void Start()
    {
        direction = (endTrans.position - startTrans.position).normalized;
    }

    void Update()
    {
        Camera.main.transform.eulerAngles = new Vector3(23 - angle, 0, 0);

        float acceletaion = Mathf.Sin(Mathf.Deg2Rad * angle) * gravity;

        currentSpeed += acceletaion * Time.deltaTime;

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
