using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    // ĳ������ �̵� �� ��� ���Ǵ� ������
    [Header("Controls")]
    public float playerSpeed = 5.0f; // �÷��̾� �̵� �ӵ�
    public float crouchSpeed = 2.0f; // �ɾ��� ���� �̵� �ӵ�
    public float sprintSpeed = 7.0f; // �޸� ���� �̵� �ӵ�
    public float jumpHeight = 0.8f; // ���� ����
    public float gravityMultiplier = 2; // �߷� ���
    public float rotationSpeed = 5f; // ĳ���� ȸ�� �ӵ�
    public float crouchColliderHeight = 1.35f; // �ɾ��� ���� �ݶ��̴� ����

    // �ִϸ��̼� �ε巯�� ��ȯ�� ���Ǵ� ������
    [Header("Animation Smoothing")]
    [Range(0, 1)]
    public float speedDampTime = 0.1f; // �ӵ� �ε巯�� ��ȯ �ð�
    [Range(0, 1)]
    public float velocityDampTime = 0.9f; // �ӵ� ���� �ε巯�� ��ȯ �ð�
    [Range(0, 1)]
    public float rotationDampTime = 0.2f; // ȸ�� �ε巯�� ��ȯ �ð�
    [Range(0, 1)]
    public float airControl = 0.5f; // ���� ����

    // ���� �ӽŰ� �� ���¸� ��Ÿ���� ������
    public StateMachine movementSM;
    public StandingState standing;
    public JumpingState jumping;
    public CrouchingState crouching;
    public LandingState landing;
    public SprintState sprinting;
    public SprintJumpState sprintjumping;
    public CombatState combatting;
    public AttackState attacking;

    // ���� �ʿ��� ������
    [HideInInspector]
    public float gravityValue = -9.81f; // �߷� ��
    [HideInInspector]
    public float normalColliderHeight; // ������ �ݶ��̴� ����
    [HideInInspector]
    public CharacterController controller; // ĳ���� ��Ʈ�ѷ�
    [HideInInspector]
    public PlayerInput playerInput; // �÷��̾� �Է�
    [HideInInspector]
    public Transform cameraTransform; // ī�޶��� Transform
    [HideInInspector]
    public Animator animator; // �ִϸ�����
    [HideInInspector]
    public Vector3 playerVelocity; // �÷��̾��� �ӵ� ����

    // Start is called before the first frame update
    private void Start()
    {
        // �ʿ��� ������Ʈ �� ���� �ʱ�ȭ
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;

        // ���� �ӽŰ� ���� ��ü �ʱ�ȭ
        movementSM = new StateMachine();
        standing = new StandingState(this, movementSM);
        jumping = new JumpingState(this, movementSM);
        crouching = new CrouchingState(this, movementSM);
        landing = new LandingState(this, movementSM);
        sprinting = new SprintState(this, movementSM);
        sprintjumping = new SprintJumpState(this, movementSM);
        combatting = new CombatState(this, movementSM);
        attacking = new AttackState(this, movementSM);

        // �ʱ� ���� ����
        movementSM.Initialize(standing);

        // ������ �ݶ��̴� ���� ����
        normalColliderHeight = controller.height;

        // �߷� �� ����
        gravityValue *= gravityMultiplier;
    }

    // Update is called once per frame
    private void Update()
    {
        // ���� ���¿� ���� �Է� ó�� �� ���� ������Ʈ ����
        movementSM.currentState.HandleInput();
        movementSM.currentState.LogicUpdate();
    }

    // FixedUpdate is called a fixed number of times per second
    private void FixedUpdate()
    {
        // ���� ���¿� ���� ���� ������Ʈ ����
        movementSM.currentState.PhysicsUpdate();
    }
}
