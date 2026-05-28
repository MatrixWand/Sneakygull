using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Vector2 input;
    private CharacterController characterController;
    private Vector3 direction;

    [SerializeField] private Animator animator;

    [SerializeField] private float smoothTime = 0.05f;
    private float currentVelocity;


    [SerializeField] private float speed;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }


    // To allow the player avatar to face whichever 
    // direction  the character is moving
    private void Update()
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeVerticalInput = input.y * forward;
        Vector3 rightRelativeVerticalInput = input.x * right;

        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeVerticalInput;

        var targetAngle = Mathf.Atan2(cameraRelativeMovement.x, cameraRelativeMovement.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, smoothTime);

        characterController.Move(cameraRelativeMovement * speed * Time.deltaTime);
        if (input.y != 0 || input.x != 0) 
        {
            animator.SetBool("IsWalking", true);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0.0f, input.y);
    }


}
