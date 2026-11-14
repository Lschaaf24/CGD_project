using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Timeline;
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

    [SerializeField] bool slidePaused = false;
    float lastAcceleration = 0;



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
            netAcceleration = gravityAcceleration - (Mathf.Sign(currentSpeed) * maxFrictionAcceleration);
        }
        else
        {
            if (Mathf.Abs(gravityAcceleration) > maxFrictionAcceleration)
            {
                netAcceleration = gravityAcceleration - (Mathf.Sign(gravityAcceleration) * maxFrictionAcceleration);
            }
            else
            {
                currentSpeed = 0f;
                netAcceleration = 0f;
            }
        }


        if (slidePaused)
        {
            if(Mathf.Sign(lastAcceleration) != Mathf.Sign(netAcceleration)) 
            {
                slidePaused = false;
            }
            else
            {
                lastAcceleration = netAcceleration;
                return;
            }
        }

        lastAcceleration = netAcceleration;
        currentSpeed += netAcceleration * Time.deltaTime;
        transform.position += direction * currentSpeed  * Time.deltaTime;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (slidePaused) return;
        if(collision.gameObject.layer == LayerMask.GetMask("Cargo")) 
        {
            PauseSlide();
            return;
        }
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;
        if (player.GetIsStunned()) return;

        Vector3 displacement = player.transform.position - transform.position;
        float dot = Vector3.Dot(displacement.normalized, direction * Mathf.Sign(currentSpeed));
        if(dot > 0.7071f && MathF.Abs(currentSpeed) > 0.75f) 
        {
            player.stunPlayer();
        }

    }

    public void PauseSlide() 
    {
        slidePaused = true;
        currentSpeed = 0;
    }


    IEnumerator LerpSquash(GameObject player) 
    {
        float timeElapsed = 0;
        player.GetComponent<Collider>().isTrigger = true;
        Vector3 startScale = player.transform.localScale;
        Vector3 startPosition = player.transform.position;
        while (timeElapsed < 0.2f) 
        {
            player.transform.localScale = Vector3.Lerp(startScale, new Vector3(2, 0.02f, 2), timeElapsed / 0.2f);
            player.transform.position = Vector3.Lerp(startPosition, new Vector3(startPosition.x, 0, startPosition.z), timeElapsed/ 0.2f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator LerpUnsquash(GameObject player) 
    {
        float timeElapsed = 0;
        player.GetComponent<Collider>().isTrigger = false;
        Vector3 startScale = player.transform.localScale;
        Vector3 startPosition = player.transform.position;
        while (timeElapsed < 0.2f)
        {
            player.transform.localScale = Vector3.Lerp(startScale, new Vector3(1, 1, 1), timeElapsed / 0.2f);
            player.transform.position = Vector3.Lerp(startPosition, new Vector3(startPosition.x, 0.5f, startPosition.z), timeElapsed / 0.2f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
