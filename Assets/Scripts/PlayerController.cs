using NUnit.Framework.Internal;
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

    private GameObject closestObject = null;
    private GameObject interactedObject = null;
    private bool inTask = false;

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
        if (closestObject == null) return;
        Task task;
        if (interactedObject == null) 
        {
            Debug.Log("INTERACT!");
            task = closestObject.GetComponent<Task>();
            if (task && !inTask)
            {
                if (task.EnterTask(this)) 
                {
                    inTask = true;
                    interactedObject = closestObject;
                }
                
            }
            return;

        }
      
        task = interactedObject.GetComponent<Task>();
        if (task != null) 
        {
            task.TaskAction();
        }
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
            closestObject = collider.gameObject;
            break;
        }

        if (!currentlyOverlapping)
        {
            closestObject = null;
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    public void TaskEject() 
    {
        inTask = false;
        interactedObject = null;
        Debug.Log("EJECTED");
    }
}
