using UnityEngine;

public class StandingState : State
{
    // �ʿ��� ������ ����
    float gravityValue; // �߷� ��
    bool jump; // ���� �Է� ����
    bool crouch; // �ɱ� �Է� ����
    Vector3 currentVelocity; // ���� �̵� �ӵ� ����
    bool grounded; // ���鿡 ��Ҵ��� ����
    bool sprint; // ������Ʈ �Է� ����
    float playerSpeed; // �÷��̾� �̵� �ӵ�

    Vector3 cVelocity; // Vector3.SmoothDamp �޼��带 ����ϱ� ���� �ӽ� ����

    // StandingState ������
    public StandingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Enter()
    {
        base.Enter();

        // �ʱ�ȭ
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

    // �Է� ó�� �޼���
    public override void HandleInput()
    {
        base.HandleInput();

        // ����, �ɱ�, ������Ʈ �Է� ó��
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

        // �̵� �Է� ó��
        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    // ���� ������Ʈ �޼���
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // �̵� �ִϸ��̼� �ӵ� ����
        character.animator.SetFloat("speed", input.magnitude, character.speedDampTime, Time.deltaTime);

        // ������Ʈ �Է� �� ������Ʈ ���·� ��ȯ
        if (sprint)
        {
            stateMachine.ChangeState(character.sprinting);
        }

        // ���� �Է� �� ���� ���·� ��ȯ
        if (jump)
        {
            stateMachine.ChangeState(character.jumping);
        }

        // �ɱ� �Է� �� �ɱ� ���·� ��ȯ
        if (crouch)
        {
            stateMachine.ChangeState(character.crouching);
        }
    }

    // ���� ������Ʈ �޼���
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // �߷� ����
        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = character.controller.isGrounded;
        if (grounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        // ���� �̵� �ӵ� ���
        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, character.velocityDampTime);
        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        // �̵� �������� ȸ��
        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Exit()
    {
        base.Exit();

        // �ʱ�ȭ
        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);

        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
