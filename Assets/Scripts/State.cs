using UnityEngine;
using UnityEngine.InputSystem;

public class State
{
    // 캐릭터와 상태 머신에 대한 참조
    public Character character;
    public StateMachine stateMachine;

    // 중력 속도, 이동 속도 및 입력 값에 대한 벡터
    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 input;

    // 입력 액션들에 대한 참조
    public InputAction moveAction;
    public InputAction lookAction;
    public InputAction jumpAction;
    public InputAction crouchAction;
    public InputAction sprintAction;
    public InputAction drawWeaponAction;
    public InputAction attackAction;

    // State 생성자
    public State(Character _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;

        // 입력 액션들에 대한 참조 설정
        moveAction = character.playerInput.actions["Move"];
        lookAction = character.playerInput.actions["Look"];
        jumpAction = character.playerInput.actions["Jump"];
        crouchAction = character.playerInput.actions["Crouch"];
        sprintAction = character.playerInput.actions["Sprint"];
        drawWeaponAction = character.playerInput.actions["DrawWeapon"];
        attackAction = character.playerInput.actions["Attack"];
    }

    // 상태 진입 시 실행되는 메서드
    public virtual void Enter()
    {
        //StateUI.instance.SetStateText(this.ToString());
        // 상태가 진입했음을 디버그 로그로 출력
        Debug.Log("Enter State: " + this.ToString());
    }

    // 입력 처리 메서드
    public virtual void HandleInput()
    {
        // 여기에 입력 처리 로직을 추가
    }

    // 로직 업데이트 메서드
    public virtual void LogicUpdate()
    {
        // 여기에 로직 업데이트 로직을 추가
    }

    // 물리 업데이트 메서드
    public virtual void PhysicsUpdate()
    {
        // 여기에 물리 업데이트 로직을 추가
    }

    // 상태 종료 시 실행되는 메서드
    public virtual void Exit()
    {
        // 여기에 상태 종료 시 필요한 로직을 추가
    }
}
