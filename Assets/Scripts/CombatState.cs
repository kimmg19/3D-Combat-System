using UnityEngine;

public class CombatState : State
{
    // 필요한 변수들 선언
    float gravityValue; // 중력 값
    Vector3 currentVelocity; // 현재 속도 벡터
    bool grounded; // 지면에 닿았는지 여부
    bool sheathWeapon; // 무기를 도약할지 여부
    float playerSpeed; // 플레이어의 이동 속도
    bool attack; // 공격 여부를 나타내는 플래그

    Vector3 cVelocity; // 벡터의 부드러운 변화를 추적하기 위한 변수

    // CombatState 생성자
    public CombatState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // 상태 진입 시 실행되는 메서드
    public override void Enter()
    {
        base.Enter();

        // 초기화
        sheathWeapon = false;
        input = Vector2.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        attack = false;

        // 필요한 변수 설정
        velocity = character.playerVelocity;
        playerSpeed = character.playerSpeed;
        grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;
    }

    // 입력 처리 메서드
    public override void HandleInput()
    {
        base.HandleInput();

        // 무기를 도약할 경우
        if (drawWeaponAction.triggered)
        {
            sheathWeapon = true;
        }

        // 공격할 경우
        if (attackAction.triggered)
        {
            attack = true;
        }

        // 이동 입력 처리
        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        // 이동 방향을 카메라 기준으로 설정
        velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    // 로직 업데이트 메서드
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 애니메이션에 속도 값을 전달하여 재생 속도 설정
        character.animator.SetFloat("speed", input.magnitude, character.speedDampTime, Time.deltaTime);

        // 무기를 도약할 경우
        if (sheathWeapon)
        {
            // 무기를 도약하는 애니메이션 재생
            character.animator.SetTrigger("sheathWeapon");
            // 서있는 상태로 전환
            stateMachine.ChangeState(character.standing);
        }

        // 공격할 경우
        if (attack)
        {
            // 공격 애니메이션 재생
            character.animator.SetTrigger("attack");
            // 공격 상태로 전환
            stateMachine.ChangeState(character.attacking);
        }
    }

    // 물리 업데이트 메서드
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 중력 적용
        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = character.controller.isGrounded;

        // 지면에 닿았을 때 중력 속도 초기화
        if (grounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        // 속도 벡터를 부드럽게 변화시킴
        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, character.velocityDampTime);
        // 캐릭터 이동
        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        // 캐릭터 회전
        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }
    }

    // 상태 종료 시 실행되는 메서드
    public override void Exit()
    {
        base.Exit();

        // 중력 속도 초기화
        gravityVelocity.y = 0f;
        // 플레이어의 속도 벡터 설정
        character.playerVelocity = new Vector3(input.x, 0, input.y);

        // 이동 중일 경우 캐릭터를 이동 방향으로 회전
        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
