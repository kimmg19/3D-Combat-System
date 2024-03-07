using UnityEngine;

public class StandingState : State
{
    // 필요한 변수들 선언
    float gravityValue; // 중력 값
    bool jump; // 점프 입력 여부
    bool crouch; // 앉기 입력 여부
    Vector3 currentVelocity; // 현재 이동 속도 벡터
    bool grounded; // 지면에 닿았는지 여부
    bool sprint; // 스프린트 입력 여부
    float playerSpeed; // 플레이어 이동 속도

    Vector3 cVelocity; // Vector3.SmoothDamp 메서드를 사용하기 위한 임시 변수

    // StandingState 생성자
    public StandingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // 상태 진입 시 실행되는 메서드
    public override void Enter()
    {
        base.Enter();

        // 초기화
        jump = false;
        crouch = false;
        sprint = false;
        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        playerSpeed = character.playerSpeed;
        grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;
    }

    // 입력 처리 메서드
    public override void HandleInput()
    {
        base.HandleInput();

        // 점프, 앉기, 스프린트 입력 처리
        if (jumpAction.triggered)
        {
            jump = true;
        }
        if (crouchAction.triggered)
        {
            crouch = true;
        }
        if (sprintAction.triggered)
        {
            sprint = true;
        }

        // 이동 입력 처리
        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    // 로직 업데이트 메서드
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 이동 애니메이션 속도 설정
        character.animator.SetFloat("speed", input.magnitude, character.speedDampTime, Time.deltaTime);

        // 스프린트 입력 시 스프린트 상태로 전환
        if (sprint)
        {
            stateMachine.ChangeState(character.sprinting);
        }

        // 점프 입력 시 점프 상태로 전환
        if (jump)
        {
            stateMachine.ChangeState(character.jumping);
        }

        // 앉기 입력 시 앉기 상태로 전환
        if (crouch)
        {
            stateMachine.ChangeState(character.crouching);
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
        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        // 이동 방향으로 회전
        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }
    }

    // 상태 종료 시 실행되는 메서드
    public override void Exit()
    {
        base.Exit();

        // 초기화
        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);

        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
