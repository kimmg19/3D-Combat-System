using UnityEngine;

public class CombatState : State
{
    // �ʿ��� ������ ����
    float gravityValue; // �߷� ��
    Vector3 currentVelocity; // ���� �ӵ� ����
    bool grounded; // ���鿡 ��Ҵ��� ����
    bool sheathWeapon; // ���⸦ �������� ����
    float playerSpeed; // �÷��̾��� �̵� �ӵ�
    bool attack; // ���� ���θ� ��Ÿ���� �÷���

    Vector3 cVelocity; // ������ �ε巯�� ��ȭ�� �����ϱ� ���� ����

    // CombatState ������
    public CombatState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Enter()
    {
        base.Enter();

        // �ʱ�ȭ
        sheathWeapon = false;
        input = Vector2.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        attack = false;

        // �ʿ��� ���� ����
        velocity = character.playerVelocity;
        playerSpeed = character.playerSpeed;
        grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;
    }

    // �Է� ó�� �޼���
    public override void HandleInput()
    {
        base.HandleInput();

        // ���⸦ ������ ���
        if (drawWeaponAction.triggered)
        {
            sheathWeapon = true;
        }

        // ������ ���
        if (attackAction.triggered)
        {
            attack = true;
        }

        // �̵� �Է� ó��
        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        // �̵� ������ ī�޶� �������� ����
        velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    // ���� ������Ʈ �޼���
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // �ִϸ��̼ǿ� �ӵ� ���� �����Ͽ� ��� �ӵ� ����
        character.animator.SetFloat("speed", input.magnitude, character.speedDampTime, Time.deltaTime);

        // ���⸦ ������ ���
        if (sheathWeapon)
        {
            // ���⸦ �����ϴ� �ִϸ��̼� ���
            character.animator.SetTrigger("sheathWeapon");
            // ���ִ� ���·� ��ȯ
            stateMachine.ChangeState(character.standing);
        }

        // ������ ���
        if (attack)
        {
            // ���� �ִϸ��̼� ���
            character.animator.SetTrigger("attack");
            // ���� ���·� ��ȯ
            stateMachine.ChangeState(character.attacking);
        }
    }

    // ���� ������Ʈ �޼���
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // �߷� ����
        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = character.controller.isGrounded;

        // ���鿡 ����� �� �߷� �ӵ� �ʱ�ȭ
        if (grounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        // �ӵ� ���͸� �ε巴�� ��ȭ��Ŵ
        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, character.velocityDampTime);
        // ĳ���� �̵�
        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        // ĳ���� ȸ��
        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Exit()
    {
        base.Exit();

        // �߷� �ӵ� �ʱ�ȭ
        gravityVelocity.y = 0f;
        // �÷��̾��� �ӵ� ���� ����
        character.playerVelocity = new Vector3(input.x, 0, input.y);

        // �̵� ���� ��� ĳ���͸� �̵� �������� ȸ��
        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
