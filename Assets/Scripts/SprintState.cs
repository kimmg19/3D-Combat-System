using UnityEngine;

public class SprintState : State
{
    // 필요한 변수들 선언
    float gravityValue; // 중력 값
    Vector3 currentVelocity; // 현재 이동 속도 벡터

    bool grounded; // 지면에 닿았는지 여부
    bool sprint; // 스프린트 중인지 여부
    float playerSpeed; // 플레이어 스프린트 속도
    bool sprintJump; // 스프린트 점프 여부
    Vector3 cVelocity; // Vector3.SmoothDamp 메서드를 사용하기 위한 임시 변수

    // SprintState 생성자
    public SprintState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // 상태 진입 시 실행되는 메서드
    public override void Enter()
    {
        base.Enter();

        // 초기화
        sprint = false;
        sprintJump = false;
        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        playerSpeed = character.sprintSpeed;
        grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;
    }

    // 입력 처리 메서드
    public override void HandleInput()
    {
        base.HandleInput();

        // 이동 입력 처리
        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity.y = 0f;

        // 스프린트 및 점프 입력 처리
        if (sprintAction.triggered || input.sqrMagnitude == 0f)
        {
            sprint = false;
        } else
        {
            sprint = true;
        }
        if (jumpAction.triggered)
        {
            sprintJump = true;
        }
    }

    // 로직 업데이트 메서드
    public override void LogicUpdate()
    {
        // 스프린트 중이라면
        if (sprint)
        {
            // 애니메이션 속도 설정
            character.animator.SetFloat("speed", input.magnitude + 0.5f, character.speedDampTime, Time.deltaTime);
        } else
        {
            // 스프린트 중이 아니라면 서있는 상태로 전환
            stateMachine.ChangeState(character.standing);
        }

        // 스프린트 점프 상태로 전환
        if (sprintJump)
        {
            stateMachine.ChangeState(character.sprintjumping);
        }
    }

    // 물리 업데이트 메서드
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 중력 적용
        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = character.controller.isGrounded;
        if (grounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        // 현재 이동 속도 계산
        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, character.velocityDampTime);

        // 캐릭터 이동
        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        // 이동 방향으로 회전
        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }
    }
}
