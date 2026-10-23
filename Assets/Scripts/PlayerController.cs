
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionRadius = 0.1f;
    [SerializeField] private LayerMask interactionLayers;

    [SerializeField] private float hopForce = 5.0f;
    [SerializeField] private float dashForce = 20.0f;
    [SerializeField] private float slideSpeed = 8f;
    [SerializeField] private float slideDuration = 1.0f;
    private float slideTimer;
    private bool isSliding = false;

    
    private Vector2 movementInput = Vector2.zero;
    private Vector3 moveDirection;
    private Rigidbody rb;

    private GameObject interactedObject = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
        if (interactedObject == null) return;
        Debug.Log("interaction");
       
    }

    public void OnSlide(InputValue value)
    {
        StartSlide();
    }

    private void StartSlide()
    {
        rb.AddForce(Vector3.up * hopForce, ForceMode.Impulse);

        Vector3 dashDirection = new Vector3(movementInput.x, 0f, movementInput.y);

        rb.AddForce(dashDirection, ForceMode.Impulse);

        slideTimer = 0.0f;
        isSliding = true;
    }

    private void Update()
    {
        InteractionDetection();


        if (isSliding)
        {
            HandleSlide();
        }
        
        
        moveDirection = new Vector3(movementInput.x, 0.0f, movementInput.y);
        rb.linearVelocity = moveDirection * moveSpeed * Time.deltaTime;
        
        
    }

    private void HandleSlide()
    {
        slideTimer += Time.fixedDeltaTime;

        if(slideTimer >= slideDuration)
        {
            isSliding = false;
            return;
        }
    }

    private void InteractionDetection()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius, interactionLayers);

        bool currentlyOverlapping = false;
        foreach (Collider collider in colliders)
        {
            currentlyOverlapping = true;
            interactedObject = collider.gameObject;
            break;
        }

        if (!currentlyOverlapping)
        {
            interactedObject = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
