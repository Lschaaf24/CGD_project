using System;
using System.Collections;
using UnityEngine;


public class ObstacleMove : MonoBehaviour
{

    [SerializeField] private Transform startTrans;
    [SerializeField] private Transform endTrans;

    [SerializeField] private LayerMask cargoLayer;


    Vector3 direction;

    float netAcceleration;
    float gravityAcceleration;


    float angle = 0.0f;

    [Range(0, 10)]
    [SerializeField] float gravity = 0.0f;

    [SerializeField] float frictionCoefficient;

    float currentSpeed = 0.0f;

    [SerializeField] bool canFall = false;

    [SerializeField] bool slidePaused = false;

    private float blockedDirection = 0f;


    void Start()
    {
        direction = (endTrans.position - startTrans.position).normalized;
    }

    void Update()
    {
        //Debug.Log(currentSpeed);
        float rawAngle = Camera.main.transform.eulerAngles.z;
        if (rawAngle > 180f) rawAngle -= 360f;
        angle = rawAngle;
        float rad = angle * Mathf.Deg2Rad;

        gravityAcceleration = Mathf.Sin(rad) * gravity;

        float maxFrictionAcceleration = Mathf.Abs(Mathf.Cos(rad)) * gravity * frictionCoefficient;

        netAcceleration = 0f;

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
                Debug.Log("FRICTION LOCK");
            }
        }

        if (slidePaused)
        {
            if (gravityAcceleration * blockedDirection < -0.05f)
            {
                slidePaused = false;
                blockedDirection = 0f;
            }
            else
            {
                return;
            }
        }

        currentSpeed += netAcceleration * Time.deltaTime;
        transform.position += direction * currentSpeed  * Time.deltaTime;

        if (!canFall) 
        {
            ClampToPath();
        }

    }

    private void HandleCollision(Collision collision) 
    {
        if((cargoLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            if(collision.contactCount > 0)
            {
                ContactPoint contact = collision.GetContact(0);
                float normalAlongPath = Vector3.Dot(contact.normal, direction);
 
                if(Mathf.Abs(normalAlongPath) > 0.5f) 
                {
                    float intoObstacleDir = -Mathf.Sign(normalAlongPath);

                    if(contact.separation < 0f) 
                    {
                        transform.position += contact.normal * (-contact.separation + 0.005f);
                    }

                    bool pushingInto = (currentSpeed * intoObstacleDir > 0.001f) || (netAcceleration * intoObstacleDir > 0.001f);

                    if(!slidePaused && pushingInto) 
                    {
                        blockedDirection = intoObstacleDir;
                        slidePaused = true;
                        currentSpeed = 0f;
                    }
                }
            }            
            return;
        }

        if (slidePaused) return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;
        if (player.GetIsStunned()) return;

        Vector3 displacement = player.transform.position - transform.position;
        float dot = Vector3.Dot(displacement.normalized, direction * Mathf.Sign(currentSpeed));
        if (dot > 0.7071f && MathF.Abs(currentSpeed) > 0.75f)
        {
            player.stunPlayer();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        HandleCollision(collision);
    }
    private void ClampToPath()
    {
        Vector3 path = endTrans.position - startTrans.position;
        float maxDistance = path.magnitude;

        float progress = Vector3.Dot(transform.position - startTrans.position, path.normalized);

        if(currentSpeed < 0f && progress <= 0f) 
        {
            transform.position = startTrans.position;
            currentSpeed = 0f;
        }
        else if( currentSpeed > 0f && progress >= maxDistance) 
        {
            transform.position = endTrans.position;
            currentSpeed = 0f;
        }
    }


    public void PauseSlide() 
    {
        slidePaused = true;

        if (Mathf.Abs(currentSpeed) > 0.001f)
            blockedDirection = Mathf.Sign(currentSpeed);
        else if (Mathf.Abs(netAcceleration) > 0.001f)
            blockedDirection = Mathf.Sign(netAcceleration);
        else
            blockedDirection = 1f;

        currentSpeed = 0;
        Debug.Log("PAUSE SLIDE");
        return;
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
