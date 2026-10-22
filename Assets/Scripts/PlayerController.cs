using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movementInput = Vector2.zero;


    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }


    private void Update()
    {
        Vector3 moveDirection = transform.right * movementInput.x + transform.forward * movementInput.y;

        

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        
    }
}
