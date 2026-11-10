using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [SerializeField] private float itemThrowHeight = 1f;
    //[SerializeField] private float itemThrowForce = 4f;
    [SerializeField] private float itemThrowSpeed = 10f;
    [SerializeField] private float stunDuration = 2f;
    [SerializeField] private GameObject stunParticlePrefab;


    private ThrowIndication throwIndication;

    private float slideTimer;
    private bool isSliding = false;
    private bool isStunned = false;

    private Item heldItem = null;

    private PlayerInput playerInput;
    private InputAction throwAction;

    private Vector2 movementInput = Vector2.zero;
    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    private Vector3 throwDirection;
    private Rigidbody rb;

    private GameObject closestObject = null;
    private GameObject interactedObject = null;

    private bool isChargingThrow;

    [SerializeField] private bool inTask = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        throwIndication = GetComponentInChildren<ThrowIndication>();
    }
    private void OnEnable()
    {
        throwAction = playerInput.actions["Throw"];
        throwAction.started += OnThrowStarted;
        throwAction.canceled += OnThrowCanceled;
    }

    private void OnDisable()
    {
        throwAction.started -= OnThrowStarted;
        throwAction.canceled -= OnThrowCanceled;
        
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
        if (throwDirection.sqrMagnitude < 0.01f)
        {
            throwDirection = throwIndication.GetCurrentDirection();
        }
        heldItem.GetComponent<Collider>().enabled = true;
        heldItem.SetThrower(this);
        heldItem.ReleaseCharge(throwDirection, itemThrowHeight, itemThrowSpeed);
        heldItem = null;
    }


    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
        lastMoveDirection = moveDirection;
    }

    public void OnInteract(InputValue value)
    {
        //if (heldItem != null) return;
        if (closestObject == null) return;
        Task task;
        Item item;
        ObstacleMove box;
        if (interactedObject == null) 
        {
            task = closestObject.GetComponent<Task>();
            item = closestObject.GetComponent<Item>();
            box = closestObject.GetComponent<ObstacleMove>();
            if (box) 
            {
                box.PauseSlide();
            }
            else if(item)
            {
                 ReceiveItem(item);
            }
            else if (task && !inTask)
            {
                if (task.EnterTask(this)) 
                {
                    if (!task.isTaskCompleted()) 
                    {
                        inTask = true;
                        interactedObject = closestObject;
                    }
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
        if (!isSliding && moveDirection.magnitude > 0.1f && !isChargingThrow)
        {
            StartCoroutine(StartSlide());
        }
    }
    
    private IEnumerator StartSlide()
    { 
        isSliding = true;

        rb.AddForce(moveDirection * dashForce + Vector3.up * hopForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f);

        slideTimer = 0.0f;
        while (slideTimer < slideDuration)
        {
            Vector3 slideDirection = lastMoveDirection * slideSpeed;
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
        if (isSliding || isStunned) return;

        if (isChargingThrow)
        {
            throwIndication.gameObject.SetActive(true);
            throwIndication.UpdateChargeMeter(heldItem.GetChargeRatio());
            throwIndication.UpdateArrowDirection(new Vector3(movementInput.x, 0, movementInput.y));
            throwDirection = new Vector3(movementInput.x, 0, movementInput.y);
            return;
        }
        else
        {
            throwIndication.gameObject.SetActive(false);
            throwIndication.UpdateChargeMeter(0f);
        }

        moveDirection = new Vector3(movementInput.x, 0.0f, movementInput.y);
        Vector3 walkingDirection = moveDirection * moveSpeed;

        Vector3 velocity = rb.linearVelocity;

        velocity = new Vector3(Mathf.Lerp(velocity.x, walkingDirection.x, 12f * Time.fixedDeltaTime), velocity.y , Mathf.Lerp(velocity.z, walkingDirection.z, 12f * Time.fixedDeltaTime));

        rb.linearVelocity = velocity;

    }


    private void InteractionDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius, interactionLayers);

        GameObject newClosest = null;
        float shortestDistancesSqr = float.MaxValue;
        int highestPriority = -1;

        foreach(Collider collider in colliders) 
        {
            GameObject obj = collider.gameObject;
            float sqrDist = (obj.transform.position - transform.position).sqrMagnitude;

            int priority = 0;
            if(obj.GetComponent<ObstacleMove>() != null) 
            {
                priority = 2;
            }
            else if(obj.GetComponent<Item>()!= null) 
            {
                priority = 1;
            }
            else if(obj.GetComponent<Task>() != null) 
            {
                priority = 0;
            }

            if(priority > highestPriority) 
            {
                highestPriority = priority;
                shortestDistancesSqr = sqrDist;
                newClosest = obj;
            }
            else if(priority == highestPriority) 
            {
                if(sqrDist < shortestDistancesSqr) 
                {
                    shortestDistancesSqr = sqrDist;
                    newClosest = obj;
                }
            }
        }

        closestObject = newClosest;

        if(closestObject == null && inTask && interactedObject != null) 
        {
            interactedObject.GetComponent<Task>()?.LeaveTask(this);
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
    }

    public void ReceiveItem(Item item)
    {
        if (heldItem != null) 
        {
            heldItem.transform.SetParent(null);
            heldItem.GetComponent<Collider>().enabled = true;

        }

        item.ResetThrowerCollision();

        item.transform.SetParent(transform, false);
        heldItem = item;
        heldItem.GetComponent<Collider>().enabled = false;

    }

    public Item SwapItem(Item item, Transform senderTransform) 
    {
        if (item != null) 
        {
            item.transform.SetParent(transform, false);
            item.GetComponent<Collider>().enabled = false;
        }
        if (heldItem != null) 
        {
            heldItem.transform.SetParent(senderTransform, false);
            heldItem.GetComponent<Collider>().enabled = true;
        }

        Item temp = heldItem;
        heldItem = item;
        




        return temp;


    }   

    public void stunPlayer()
    {
        if (!isStunned)
        {
            StartCoroutine(StunCoroutine(stunDuration));
        }
    }

    private IEnumerator StunCoroutine(float duration)
    {
        Instantiate(stunParticlePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        float timer = 0f;
        while (timer < duration)
        {
            isStunned = true;
            timer += Time.deltaTime;
            yield return null;
        }
        isStunned = false;
    }

    public Item GetHeldItem() 
    {
        return heldItem;
    }

    public Item TakeItem() 
    {
        Item item = heldItem;
        heldItem = null;
        return item;
    }

    public bool GetIsStunned() 
    {
        return isStunned;
    }
}
