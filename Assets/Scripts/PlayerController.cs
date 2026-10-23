
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float interactionRadius = 0.1f;
    [SerializeField] private LayerMask interactionLayers;

    [SerializeField] private float hopForce = 0.5f;
    [SerializeField] private float dashForce = 1f;
    [SerializeField] private float slideSpeed = 2f;
    [SerializeField] private float slideDuration = 1.0f;
    [SerializeField] private float slideCooldown = 0.5f;
    private float slideTimer;
    private bool isSliding = false;

    private Item heldItem = null;
    [SerializeField] private InputActionReference Throw;


    private Vector2 movementInput = Vector2.zero;
    private Vector3 moveDirection;
    private Rigidbody rb;

    private GameObject interactedObject = null;
    private GameObject itemObject = null;

    private bool isChargingThrow;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        Throw.action.started += OnThrowStarted;
        Throw.action.canceled += OnThrowCanceled;
    }

    private void OnDisable()
    {
        Throw.action.started -= OnThrowStarted;
        Throw.action.canceled -= OnThrowCanceled;
    }

    private void OnThrowStarted(InputAction.CallbackContext context)
    {
        if (heldItem == null) return;

        isChargingThrow = true;
        heldItem.BeginCharge();
    }

    private void OnThrowCanceled(InputAction.CallbackContext context)
    {
        if (heldItem == null || !isChargingThrow) return;

        isChargingThrow = false;
        heldItem.ReleaseCharge(transform.forward);
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

    public void OnPickup(InputValue value)
    {
        if (itemObject == null) return;
        Debug.Log("pickup");

        itemObject.transform.SetParent(this.gameObject.transform, false);

    }

    public void OnSlide(InputValue value)
    {
        if (!isSliding)
        {
            StartCoroutine(StartSlide());
        }
    }
    
    //public void OnThrow(InputValue value)
    //{
    //    bool isPressed = value.isPressed;

    //    if (heldItem == null) return;

    //    if (isPressed && !isChargingThrow)
    //    {
    //        isChargingThrow = true;
    //        heldItem.BeginCharge();
    //    }
    //    else if (!isPressed && isChargingThrow)
    //    {
    //        isChargingThrow = false;
    //        heldItem.ReleaseCharge(transform.forward);
    //    }
    //}

    private IEnumerator StartSlide()
    {
        isSliding = true;


        //rb.AddForce(Vector3.up * hopForce, ForceMode.Impulse);
        //rb.AddForce(moveDirection * slideSpeed, ForceMode.Impulse);

        rb.AddForce(moveDirection * dashForce + Vector3.up * hopForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f);

        slideTimer = 0.0f;
        while (slideTimer < slideDuration)
        {
            Vector3 slideDirection = moveDirection * slideSpeed;
            Vector3 velocity = rb.linearVelocity;
            velocity = new Vector3(Mathf.Lerp(velocity.x, slideDirection.x, 12f * Time.fixedDeltaTime), velocity.y, Mathf.Lerp(velocity.z, slideDirection.z, 12f * Time.fixedDeltaTime));
            rb.linearVelocity = velocity;

            slideTimer += Time.deltaTime;

            yield return null;
        }

        
        yield return new WaitForSeconds(slideCooldown);
        isSliding = false;
    }

    private void FixedUpdate()
    {
        InteractionDetection();

        if (isSliding) return; 

        moveDirection = new Vector3(movementInput.x, 0.0f, movementInput.y);
        Vector3 walkingDirection = moveDirection * moveSpeed;

        Vector3 velocity = rb.linearVelocity;

        velocity = new Vector3(Mathf.Lerp(velocity.x, walkingDirection.x, 12f * Time.fixedDeltaTime), velocity.y , Mathf.Lerp(velocity.z, walkingDirection.z, 12f * Time.fixedDeltaTime));

        rb.linearVelocity = velocity;

    }


    private void InteractionDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius, interactionLayers);

        bool currentlyOverlapping = false;
        foreach (Collider collider in colliders)
        {
            currentlyOverlapping = true;

            if (collider.gameObject.GetComponent<Item>() != null){
                itemObject = collider.gameObject;
                heldItem = collider.gameObject.GetComponent<Item>();
            }
            
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
