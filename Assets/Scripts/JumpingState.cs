using UnityEngine;

public class JumpingState : State
{
    // �ʿ��� ������ ����
    bool grounded; // ���鿡 ��Ҵ��� ����
    float gravityValue; // �߷� ��
    float jumpHeight; // ���� ����
    float playerSpeed; // �÷��̾� �̵� �ӵ�

    Vector3 airVelocity; // ���߿����� �̵� �ӵ� ����

    // JumpingState ������
    public JumpingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Enter()
    {
        base.Enter();

        // �ʱ�ȭ
        grounded = false;
        gravityValue = character.gravityValue;
        jumpHeight = character.jumpHeight;
        playerSpeed = character.playerSpeed;
        gravityVelocity.y = 0;

        // �ִϸ��̼� ��� �� ����
        character.animator.SetFloat("speed", 0);
        character.animator.SetTrigger("jump");
        Jump();
    }

    // �Է� ó�� �޼���
    public override void HandleInput()
    {
        base.HandleInput();

        // �̵� �Է� ó��
        input = moveAction.ReadValue<Vector2>();
    }

    // ���� ������Ʈ �޼���
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ���鿡 ������ ���� ���·� ��ȯ
        if (grounded)
        {
            stateMachine.ChangeState(character.landing);
        }
    }

    // ���� ������Ʈ �޼���
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // ���߿� �ִ� ���
        if (!grounded)
        {
            // �̵� �ӵ� ����
            velocity = character.playerVelocity;
            airVelocity = new Vector3(input.x, 0, input.y);

            velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
            velocity.y = 0f;
            airVelocity = airVelocity.x * character.cameraTransform.right.normalized + airVelocity.z * character.cameraTransform.forward.normalized;
            airVelocity.y = 0f;

            // ĳ���� �̵�
            character.controller.Move(gravityVelocity * Time.deltaTime + (airVelocity * character.airControl + velocity * (1 - character.airControl)) * playerSpeed * Time.deltaTime);
        }

        // �߷� ����
        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = character.controller.isGrounded;
    }

    // ���� �޼���
    void Jump()
    {
        // �ʱ� ���� �ӵ� ����Ͽ� ����
        gravityVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }
}
