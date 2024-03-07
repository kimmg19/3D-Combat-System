using UnityEngine;

public class SprintState : State
{
    // �ʿ��� ������ ����
    float gravityValue; // �߷� ��
    Vector3 currentVelocity; // ���� �̵� �ӵ� ����

    bool grounded; // ���鿡 ��Ҵ��� ����
    bool sprint; // ������Ʈ ������ ����
    float playerSpeed; // �÷��̾� ������Ʈ �ӵ�
    bool sprintJump; // ������Ʈ ���� ����
    Vector3 cVelocity; // Vector3.SmoothDamp �޼��带 ����ϱ� ���� �ӽ� ����

    // SprintState ������
    public SprintState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Enter()
    {
        base.Enter();

        // �ʱ�ȭ
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

    // �Է� ó�� �޼���
    public override void HandleInput()
    {
        base.HandleInput();

        // �̵� �Է� ó��
        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity.y = 0f;

        // ������Ʈ �� ���� �Է� ó��
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

    // ���� ������Ʈ �޼���
    public override void LogicUpdate()
    {
        // ������Ʈ ���̶��
        if (sprint)
        {
            // �ִϸ��̼� �ӵ� ����
            character.animator.SetFloat("speed", input.magnitude + 0.5f, character.speedDampTime, Time.deltaTime);
        } else
        {
            // ������Ʈ ���� �ƴ϶�� ���ִ� ���·� ��ȯ
            stateMachine.ChangeState(character.standing);
        }

        // ������Ʈ ���� ���·� ��ȯ
        if (sprintJump)
        {
            stateMachine.ChangeState(character.sprintjumping);
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

        // ĳ���� �̵�
        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        // �̵� �������� ȸ��
        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }
    }
}
