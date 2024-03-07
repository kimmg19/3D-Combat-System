using UnityEngine;

public class CrouchingState : State
{
    // 필요한 변수들 선언
    float playerSpeed; // 플레이어 이동 속도
    bool belowCeiling; // 천장 아래에 있는지 여부
    bool crouchHeld; // 앉기 버튼을 누르고 있는지 여부

    bool grounded; // 지면에 닿았는지 여부
    float gravityValue; // 중력 값
    Vector3 currentVelocity; // 현재 속도 벡터

    // CrouchingState 생성자
    public CrouchingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // 상태 진입 시 실행되는 메서드
    public override void Enter()
    {
        base.Enter();

        // 애니메이션 재생 및 변수 초기화
        character.animator.SetTrigger("crouch");
        belowCeiling = false;
        crouchHeld = false;
        gravityVelocity.y = 0;

        // 이동 속도 및 콜라이더 설정
        playerSpeed = character.crouchSpeed;
        character.controller.height = character.crouchColliderHeight;
        character.controller.center = new Vector3(0f, character.crouchColliderHeight / 2f, 0f);
        grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;
    }

    // 상태 종료 시 실행되는 메서드
    public override void Exit()
    {
        base.Exit();

        // 콜라이더 설정 초기화
        character.controller.height = character.normalColliderHeight;
        character.controller.center = new Vector3(0f, character.normalColliderHeight / 2f, 0f);
        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);
        character.animator.SetTrigger("move");
    }

    // 입력 처리 메서드
    public override void HandleInput()
    {
        base.HandleInput();

        // 앉기 버튼을 누를 경우
        if (crouchAction.triggered && !belowCeiling)
        {
            crouchHeld = true;
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

        // 애니메이션 재생 및 상태 전환
        character.animator.SetFloat("speed", input.magnitude, character.speedDampTime, Time.deltaTime);
        if (crouchHeld)
        {
            stateMachine.ChangeState(character.standing);
        }
    }

    // 물리 업데이트 메서드
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 천장 아래 충돌 체크 및 중력 적용
        belowCeiling = CheckCollisionOverlap(character.transform.position + Vector3.up * character.normalColliderHeight);
        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = character.controller.isGrounded;
        if (grounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        // 속도 벡터 부드럽게 변화
        currentVelocity = Vector3.Lerp(currentVelocity, velocity, character.velocityDampTime);
        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        // 캐릭터 회전
        if (velocity.magnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }
    }

    // 천장과의 충돌 체크하는 메서드
    public bool CheckCollisionOverlap(Vector3 targetPositon)
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit hit;

        Vector3 direction = targetPositon - character.transform.position;
        if (Physics.Raycast(character.transform.position, direction, out hit, character.normalColliderHeight, layerMask))
        {
            Debug.DrawRay(character.transform.position, direction * hit.distance, Color.yellow);
            return true;
        } else
        {
            Debug.DrawRay(character.transform.position, direction * character.normalColliderHeight, Color.white);
            return false;
        }
    }
}
