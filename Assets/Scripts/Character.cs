using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [SerializeField] Transform characterBody; // �÷��̾� ĳ������ ��ü(Transform)
    [SerializeField] Transform followCam; // ���󰡴� ī�޶�(Transform)

    Vector2 moveInput; // �̵� �Է�
    bool attackInput;
    CharacterController characterController; // ĳ���� ��Ʈ�ѷ�
    Animator animator; // �ִϸ�����
    bool isRunning; // �޸����� ����
    bool isDodging; // ȸ�� ������ ����
    float turnSmoothVelocity;
    public float playerSpeed = 5f; // �÷��̾� �̵� �ӵ�
    public float sprintSpeed = 1.5f; // �÷��̾� �޸��� �ӵ�
    public float smoothDampTime = 0.15f; // ȸ�� �� �ε巯�� ���� �ð�
    float gravity = -9.8f; // �߷�
    Vector3 velocity; // ĳ������ �ӵ�

    void Start()
    {
        // �ʿ��� ������Ʈ �ʱ�ȭ
        characterController = characterBody.GetComponent<CharacterController>();
        animator = characterBody.GetComponent<Animator>();
    }

    void Update()
    {
        timeSinceAttack += Time.deltaTime;
        Attack();
        Move(); // �̵� �޼��� ȣ��
        ApplyGravity(); // �߷� ����
    }

    void Move()
    {
        if (isAttacking) return;

        // �̵� �� �޸��� ���¸� �ִϸ����Ϳ� ����
        float speed = isRunning ? sprintSpeed : 1f;
        animator.SetFloat("speed", moveInput.magnitude * speed, 0.1f, Time.deltaTime);
        if (moveInput.magnitude != 0)
        {
            // ī�޶� ���� �̵� ���� ����
            Vector3 lookForward = new Vector3(followCam.forward.x, 0f, followCam.forward.z).normalized;
            Vector3 lookRight = new Vector3(followCam.right.x, 0f, followCam.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            // ĳ������ ȸ�� ���� ���
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float currentAngle = Mathf.SmoothDampAngle(characterBody.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothDampTime);
            characterBody.rotation = Quaternion.Euler(0f, currentAngle, 0f);

            // �̵� �ӵ��� ���� ĳ���� �̵�
            characterController.Move(moveDir * Time.deltaTime * playerSpeed * speed);
        }

    }

    void ApplyGravity()
    {
        // ĳ���Ͱ� ���� ���� ������ �߷� ����
        if (!characterController.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        } else
        {
            // ���� ������� �� ���� �ӵ� �ʱ�ȭ
            velocity.y = -0.5f;
        }
        characterController.Move(velocity * Time.deltaTime);
    }

    // �̵� �Է��� ó���ϴ� �Լ�
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // �޸��� �Է��� ó���ϴ� �Լ�
    void OnSprint()
    {
        isRunning = !isRunning; // �޸��� ���� ����
    }

    // ȸ�� �Է��� ó���ϴ� �Լ�
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

    // ȸ�� �ִϸ��̼� ���� �� ȣ��Ǵ� �̺�Ʈ �Լ�
    void EndDodge()
    {
        isDodging = false;
        characterController.center = new Vector3(0, 0.88f, 0);
        characterController.height = 1.6f;
    }

    void OnAttack()
    {
        isAttacking=!isAttacking;
    }
    bool isAttacking;
    float timeSinceAttack;
    int currentAttack=0;

    void Attack()
    {
        if (Input.GetMouseButtonDown(0)&&timeSinceAttack>0.8f&& characterController.isGrounded)
        {
            print("���� ��");
            currentAttack++;
            //isAttacking = true;
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
        isAttacking=false;
    }
}