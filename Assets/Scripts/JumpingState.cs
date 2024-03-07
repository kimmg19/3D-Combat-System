using UnityEngine;

public class JumpingState : State
{
    // 필요한 변수들 선언
    bool grounded; // 지면에 닿았는지 여부
    float gravityValue; // 중력 값
    float jumpHeight; // 점프 높이
    float playerSpeed; // 플레이어 이동 속도

    Vector3 airVelocity; // 공중에서의 이동 속도 벡터

    // JumpingState 생성자
    public JumpingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // 상태 진입 시 실행되는 메서드
    public override void Enter()
    {
        base.Enter();

        // 초기화
        grounded = false;
        gravityValue = character.gravityValue;
        jumpHeight = character.jumpHeight;
        playerSpeed = character.playerSpeed;
        gravityVelocity.y = 0;

        // 애니메이션 재생 및 점프
        character.animator.SetFloat("speed", 0);
        character.animator.SetTrigger("jump");
        Jump();
    }

    // 입력 처리 메서드
    public override void HandleInput()
    {
        base.HandleInput();

        // 이동 입력 처리
        input = moveAction.ReadValue<Vector2>();
    }

    // 로직 업데이트 메서드
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 지면에 닿으면 착지 상태로 전환
        if (grounded)
        {
            stateMachine.ChangeState(character.landing);
        }
    }

    // 물리 업데이트 메서드
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 공중에 있는 경우
        if (!grounded)
        {
            // 이동 속도 설정
            velocity = character.playerVelocity;
            airVelocity = new Vector3(input.x, 0, input.y);

            velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
            velocity.y = 0f;
            airVelocity = airVelocity.x * character.cameraTransform.right.normalized + airVelocity.z * character.cameraTransform.forward.normalized;
            airVelocity.y = 0f;

            // 캐릭터 이동
            character.controller.Move(gravityVelocity * Time.deltaTime + (airVelocity * character.airControl + velocity * (1 - character.airControl)) * playerSpeed * Time.deltaTime);
        }

        // 중력 적용
        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = character.controller.isGrounded;
    }

    // 점프 메서드
    void Jump()
    {
        // 초기 점프 속도 계산하여 적용
        gravityVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }
}
