using UnityEngine;
using UnityEngine.InputSystem;

public class State
{
    // ĳ���Ϳ� ���� �ӽſ� ���� ����
    public Character character;
    public StateMachine stateMachine;

    // �߷� �ӵ�, �̵� �ӵ� �� �Է� ���� ���� ����
    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 input;

    // �Է� �׼ǵ鿡 ���� ����
    public InputAction moveAction;
    public InputAction lookAction;
    public InputAction jumpAction;
    public InputAction crouchAction;
    public InputAction sprintAction;
    public InputAction drawWeaponAction;
    public InputAction attackAction;

    // State ������
    public State(Character _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;

        // �Է� �׼ǵ鿡 ���� ���� ����
        moveAction = character.playerInput.actions["Move"];
        lookAction = character.playerInput.actions["Look"];
        jumpAction = character.playerInput.actions["Jump"];
        crouchAction = character.playerInput.actions["Crouch"];
        sprintAction = character.playerInput.actions["Sprint"];
        drawWeaponAction = character.playerInput.actions["DrawWeapon"];
        attackAction = character.playerInput.actions["Attack"];
    }

    // ���� ���� �� ����Ǵ� �޼���
    public virtual void Enter()
    {
        //StateUI.instance.SetStateText(this.ToString());
        // ���°� ���������� ����� �α׷� ���
        Debug.Log("Enter State: " + this.ToString());
    }

    // �Է� ó�� �޼���
    public virtual void HandleInput()
    {
        // ���⿡ �Է� ó�� ������ �߰�
    }

    // ���� ������Ʈ �޼���
    public virtual void LogicUpdate()
    {
        // ���⿡ ���� ������Ʈ ������ �߰�
    }

    // ���� ������Ʈ �޼���
    public virtual void PhysicsUpdate()
    {
        // ���⿡ ���� ������Ʈ ������ �߰�
    }

    // ���� ���� �� ����Ǵ� �޼���
    public virtual void Exit()
    {
        // ���⿡ ���� ���� �� �ʿ��� ������ �߰�
    }
}
