using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionRadius = 0.1f;
    [SerializeField] private LayerMask interactionLayers;

    [SerializeField] private float slideSpeed = 8f;
    [SerializeField] private float slideDuration = 1.0f;
    private bool isSliding = false;

    
    private Vector2 movementInput = Vector2.zero;
    private Vector3 playerVelocity;
    private CharacterController controller;

    private GameObject interactedObject = null;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
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
        StartCoroutine(HandleSlide());
    }

    private IEnumerator HandleSlide()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        Vector3 move = new Vector3(movementInput.x, 0.0f, movementInput.y);
        move = Vector3.ClampMagnitude(move, 1f);

        playerVelocity.y += -9.81f * Time.deltaTime;

        Vector3 finalMove = move * moveSpeed + Vector3.up * playerVelocity.y;


        controller.Move(finalMove * Time.deltaTime);




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
