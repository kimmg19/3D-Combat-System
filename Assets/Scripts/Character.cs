using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [SerializeField] Transform characterBody;
    [SerializeField] Transform followCam;
    Vector2 moveInput;
    bool attackInput;
    CharacterController characterController;
    Animator animator;
    bool isRunning;
    bool isDodging;
    float turnSmoothVelocity;
    public float playerSpeed = 5f;
    public float sprintSpeed = 1.5f;
    public float smoothDampTime = 0.15f;
    float gravity = -9.8f;
    Vector3 velocity;
    float timeSinceAttack; // 추가: 시간 추적을 위한 변수
    bool isAttacking = false; // 추가: 공격 상태를 추적하는 변수
    public int currentAttack = 0;

    void Start()
    {
        characterController = characterBody.GetComponent<CharacterController>();
        animator = characterBody.GetComponent<Animator>();
    }

    void Update()
    {
        timeSinceAttack += Time.deltaTime;
        Attack();
        Move();
        ApplyGravity();
    }

    void Move()
    {
        if (isAttacking || !isDodging) return;

        float speed = isRunning ? sprintSpeed : 1f;
        animator.SetFloat("speed", moveInput.magnitude * speed, 0.1f, Time.deltaTime);
        if (moveInput.magnitude != 0)
        {
            Vector3 lookForward = new Vector3(followCam.forward.x, 0f, followCam.forward.z).normalized;
            Vector3 lookRight = new Vector3(followCam.right.x, 0f, followCam.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float currentAngle = Mathf.SmoothDampAngle(characterBody.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothDampTime);
            characterBody.rotation = Quaternion.Euler(0f, currentAngle, 0f);

            characterController.Move(moveDir * Time.deltaTime * playerSpeed * speed);
        }
    }

    void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        } else
        {
            velocity.y = -0.5f;
        }
        characterController.Move(velocity * Time.deltaTime);
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnSprint()
    {
        isRunning = !isRunning;
    }

    void OnRoll()
    {
        if (moveInput.magnitude != 0 && !isDodging)
        {
            isDodging = true;
            animator.SetTrigger("Dodge");
            characterController.center = new Vector3(0, 0.5f, 0);
            characterController.height = 1f;
        }
    }

    void EndDodge()
    {
        characterController.center = new Vector3(0, 0.88f, 0);
        characterController.height = 1.6f;
        isDodging = false;
        isAttacking = false; // 수정: 공격 상태 초기화
        attackInput = false; // 수정: 공격 입력 초기화
    }

    void OnAttack()
    {
        attackInput = !attackInput;
    }

    void Attack()
    {
        if (attackInput && timeSinceAttack > 0.8f && characterController.isGrounded && !isDodging)
        {
            currentAttack++;
            isAttacking = true;

            if (currentAttack > 3)
                currentAttack = 1;

            if (timeSinceAttack > 1.0f)
                currentAttack = 1;
            animator.SetTrigger("Attack" + currentAttack);
            timeSinceAttack = 0;
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
        attackInput = false;
    }
}
