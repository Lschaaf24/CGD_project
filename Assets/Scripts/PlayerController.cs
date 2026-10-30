using System.Collections;
using UnityEngine;
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

    [SerializeField] private float itemThrowHeight = 1f;
    [SerializeField] private float itemThrowForce = 4f;
    [SerializeField] private float itemThrowSpeed = 10f;


    private float slideTimer;
    private bool isSliding = false;
    

    private Item heldItem = null;

    private PlayerInput playerInput;
    private InputAction throwAction;


    private Vector2 movementInput = Vector2.zero;
    private Vector3 moveDirection;
    private Rigidbody rb;

    private GameObject closestObject = null;
    private GameObject interactedObject = null;
    private GameObject itemObject = null;

    private bool isChargingThrow;

    [SerializeField] private bool inTask = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
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
        heldItem.ReleaseCharge(moveDirection * itemThrowForce + Vector3.up * itemThrowHeight, itemThrowSpeed);
        heldItem = null;
    }


    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
        Debug.Log("INTERACT START!");
        //if (heldItem != null) return;
        if (closestObject == null) return;
        Task task;
        if (interactedObject == null) 
        {

            if (itemObject != null)
            {
                Debug.Log("pickup");
                ReceiveItem(itemObject);

            }

            Debug.Log("INTERACT!");
            task = closestObject.GetComponent<Task>();
            if (task && !inTask)
            {
                Debug.Log("INTERACT!");
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
        if (!isSliding && moveDirection.magnitude > 0.1f)
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

            closestObject = collider.gameObject;

            if (collider.gameObject.GetComponent<Item>() != null)
            {
                itemObject = collider.gameObject;
            }

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

    public void ReceiveItem(Item item)
    {
        if (heldItem) 
        {
            heldItem.transform.SetParent(null);
            heldItem.GetComponent<Collider>().enabled = true;

        }
        item.transform.SetParent(transform, false);
        heldItem = item;
        heldItem.GetComponent<Collider>().enabled = false;

    }

    public Item SwapItem(Item item, Transform senderTransform) 
    {
        Debug.Log("SWAP!!!!!");
        if (item) 
        {
            item.transform.SetParent(transform, false);
        }
        if (heldItem) 
        {
            heldItem.transform.SetParent(senderTransform, false);
        }

        Item temp = heldItem;
        heldItem = item;

        return temp;


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
}
