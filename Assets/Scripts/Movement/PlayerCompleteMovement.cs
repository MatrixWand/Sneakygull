using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerCompleteMovement : MonoBehaviour
{
    private Vector2 input;
    private float verticalInput;

    private CharacterController characterController;

    [SerializeField] private Animator animator;

    [SerializeField] private float smoothTime = 0.05f;
    private float currentVelocity;

    [SerializeField] private float speed;
    [SerializeField] private float flyingSpeed = 7f;

    //Gravity 
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundedGravity = -2f;

    private float verticalVelocity;

    // Attack Damage Settings
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private LayerMask attackLayer;

    // Ground Attack
    [SerializeField] private float peckRange = 1.5f;
    [SerializeField] private int peckDamage = 1;

    // Fly Attack
    [SerializeField] private float poopRange = 2f;
    [SerializeField] private int poopDamage = 5;
    [SerializeField] private GameObject poopPrefab;
    [SerializeField] private Transform poopSpawnPoint;
    [SerializeField] private GameObject poopTarget;
    [SerializeField] private float maxTargetDistance = 50f;

    // Stamina
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float staminaDrainRate = 1f;
    [SerializeField] private float staminaRegenRate = 1.5f;

    //Stamina Bar
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider staminaLagSlider;
    [SerializeField] private Image fillImage;

    private float lagStamina;
    [SerializeField] private float lagSpeed = 2f;



    private float currentStamina;

    private float lastAttackTime;

    //Fly + Attack state
    private bool isFlying;
    private bool isAttacking;
    private bool isFalling;

    private Transform cameraTransform;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        
        currentStamina = maxStamina;

        lagStamina = currentStamina;

    }

    private void Update()
    {

        bool blockMovement = isAttacking && !isFlying;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeVerticalInput = input.y * forward;
        Vector3 rightRelativeHorizontalInput = input.x * right;

        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeHorizontalInput;


        bool isGrounded = characterController.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = groundedGravity;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }


        if (isFlying)
        {
            verticalVelocity = 0f;
        }

        if (!isFlying && !isGrounded && verticalVelocity < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }

        if (isFlying)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isFlying = false; 
            }
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }


        float movementAmount;

        if (isFlying)
        {
            Vector3 flyingMovement = cameraRelativeMovement + Vector3.up * verticalInput;

            characterController.Move(flyingMovement * flyingSpeed * Time.deltaTime);

            movementAmount = flyingMovement.magnitude;

            if (movementAmount > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(flyingMovement);
            }

            animator.SetBool("IsFlying", true);
            animator.SetBool("IsWalking", false);
        }


        else
        {

            if (!blockMovement)
            {
                Vector3 horizontalMovement = cameraRelativeMovement * speed;
                Vector3 verticalMovement = Vector3.up * verticalVelocity;

                Vector3 finalMovement = horizontalMovement + verticalMovement;

                characterController.Move(finalMovement * Time.deltaTime);
            }

            movementAmount = input.magnitude;

            if (movementAmount > 0.01f)
            {
                float targetAngle = Mathf.Atan2(cameraRelativeMovement.x, cameraRelativeMovement.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, smoothTime);

                transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
            }

            bool isMoving = input.magnitude > 0.01f;

            if (isFlying)
            {
                animator.SetBool("IsFlying", true);
                animator.SetBool("IsFalling", false);
                animator.SetBool("IsWalking", false);
            }
            else if (isFalling)
            {
                animator.SetBool("IsFlying", false);
                animator.SetBool("IsFalling", true);
                animator.SetBool("IsWalking", false);
            }
            else
            {
                animator.SetBool("IsFlying", false);
                animator.SetBool("IsFalling", false);
                animator.SetBool("IsWalking", isMoving && !isAttacking);
            }

            animator.SetFloat("Speed", movementAmount);

            if (staminaSlider != null)
                staminaSlider.value = currentStamina / maxStamina;

            
            if (staminaLagSlider != null)
            {
                lagStamina = Mathf.MoveTowards(lagStamina, currentStamina, lagSpeed * Time.deltaTime);
                staminaLagSlider.value = lagStamina / maxStamina;
            }


            if (isFlying)
            {
                Ray ray = new Ray(transform.position, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, maxTargetDistance))
                {
                    poopTarget.SetActive(true);


                    poopTarget.transform.position = Vector3.Lerp(poopTarget.transform.position, hit.point, Time.deltaTime * 10f);
                }
                else
                {
                    poopTarget.SetActive(false);
                }
            }
            else
            {
                poopTarget.SetActive(false);
            }

        }
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;  
            staminaSlider.value = currentStamina;  
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    public void FlyUpDown(InputAction.CallbackContext context)
    {
        verticalInput = context.ReadValue<float>();
    }

    public void ToggleFly(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            if (!isFlying && currentStamina < 1.0f)
                return;

            isFlying = !isFlying;
            verticalInput = 0.0f;
        }
    }

    private void SpawnPoop()
    {
        GameObject poop = Instantiate(poopPrefab, poopSpawnPoint.position, Quaternion.Euler(-90f, 0f, 0f));

        Rigidbody rb = poop.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.down * 10f; 
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            isAttacking = true;

            if (isFlying)
            {
                animator.SetTrigger("Attack");

                SpawnPoop();

                Invoke(nameof(EndAttack), 0.5f); 
            }
            else
            {
                animator.SetTrigger("Attack");

                PerformPeckAttack();
            }
        }
    }

    private void PerformPeckAttack()
    {
        Vector3 attackOrigin = transform.position + transform.forward * 1.5f;

        
        Collider[] hitObjects = Physics.OverlapSphere(attackOrigin, 2f);

        HashSet<Damageable> hitTargets = new HashSet<Damageable>();

        foreach (Collider hit in hitObjects)
        {
            Damageable damageable = hit.GetComponentInParent<Damageable>();

            if (damageable != null)
            {
                hitTargets.Add(damageable);
                damageable.TakeDamage(1);
            }
        }
    }

    private void PerformPoopAttack()
    {
        Vector3 attackOrigin = transform.position + Vector3.down * 1.0f;

        Collider[] hitObjects = Physics.OverlapSphere(attackOrigin, poopRange, attackLayer);

        foreach (Collider hit in hitObjects)
        {
            Damageable damageable = hit.GetComponent<Damageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(poopDamage);
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }
  
    }



