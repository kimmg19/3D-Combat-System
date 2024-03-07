using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    // 캐릭터의 이동 및 제어에 사용되는 변수들
    [Header("Controls")]
    public float playerSpeed = 5.0f; // 플레이어 이동 속도
    public float crouchSpeed = 2.0f; // 앉았을 때의 이동 속도
    public float sprintSpeed = 7.0f; // 달릴 때의 이동 속도
    public float jumpHeight = 0.8f; // 점프 높이
    public float gravityMultiplier = 2; // 중력 계수
    public float rotationSpeed = 5f; // 캐릭터 회전 속도
    public float crouchColliderHeight = 1.35f; // 앉았을 때의 콜라이더 높이

    // 애니메이션 부드러운 전환에 사용되는 변수들
    [Header("Animation Smoothing")]
    [Range(0, 1)]
    public float speedDampTime = 0.1f; // 속도 부드러운 전환 시간
    [Range(0, 1)]
    public float velocityDampTime = 0.9f; // 속도 벡터 부드러운 전환 시간
    [Range(0, 1)]
    public float rotationDampTime = 0.2f; // 회전 부드러운 전환 시간
    [Range(0, 1)]
    public float airControl = 0.5f; // 공중 제어

    // 상태 머신과 각 상태를 나타내는 변수들
    public StateMachine movementSM;
    public StandingState standing;
    public JumpingState jumping;
    public CrouchingState crouching;
    public LandingState landing;
    public SprintState sprinting;
    public SprintJumpState sprintjumping;
    public CombatState combatting;
    public AttackState attacking;

    // 각종 필요한 변수들
    [HideInInspector]
    public float gravityValue = -9.81f; // 중력 값
    [HideInInspector]
    public float normalColliderHeight; // 보통의 콜라이더 높이
    [HideInInspector]
    public CharacterController controller; // 캐릭터 컨트롤러
    [HideInInspector]
    public PlayerInput playerInput; // 플레이어 입력
    [HideInInspector]
    public Transform cameraTransform; // 카메라의 Transform
    [HideInInspector]
    public Animator animator; // 애니메이터
    [HideInInspector]
    public Vector3 playerVelocity; // 플레이어의 속도 벡터

    // Start is called before the first frame update
    private void Start()
    {
        // 필요한 컴포넌트 및 변수 초기화
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;

        // 상태 머신과 상태 객체 초기화
        movementSM = new StateMachine();
        standing = new StandingState(this, movementSM);
        jumping = new JumpingState(this, movementSM);
        crouching = new CrouchingState(this, movementSM);
        landing = new LandingState(this, movementSM);
        sprinting = new SprintState(this, movementSM);
        sprintjumping = new SprintJumpState(this, movementSM);
        combatting = new CombatState(this, movementSM);
        attacking = new AttackState(this, movementSM);

        // 초기 상태 설정
        movementSM.Initialize(standing);

        // 보통의 콜라이더 높이 설정
        normalColliderHeight = controller.height;

        // 중력 값 설정
        gravityValue *= gravityMultiplier;
    }

    // Update is called once per frame
    private void Update()
    {
        // 현재 상태에 따라 입력 처리 및 로직 업데이트 실행
        movementSM.currentState.HandleInput();
        movementSM.currentState.LogicUpdate();
    }

    // FixedUpdate is called a fixed number of times per second
    private void FixedUpdate()
    {
        // 현재 상태에 따라 물리 업데이트 실행
        movementSM.currentState.PhysicsUpdate();
    }
}
