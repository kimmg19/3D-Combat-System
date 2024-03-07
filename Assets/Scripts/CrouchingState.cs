using UnityEngine;

public class CrouchingState : State
{
    // �ʿ��� ������ ����
    float playerSpeed; // �÷��̾� �̵� �ӵ�
    bool belowCeiling; // õ�� �Ʒ��� �ִ��� ����
    bool crouchHeld; // �ɱ� ��ư�� ������ �ִ��� ����

    bool grounded; // ���鿡 ��Ҵ��� ����
    float gravityValue; // �߷� ��
    Vector3 currentVelocity; // ���� �ӵ� ����

    // CrouchingState ������
    public CrouchingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Enter()
    {
        base.Enter();

        // �ִϸ��̼� ��� �� ���� �ʱ�ȭ
        character.animator.SetTrigger("crouch");
        belowCeiling = false;
        crouchHeld = false;
        gravityVelocity.y = 0;

        // �̵� �ӵ� �� �ݶ��̴� ����
        playerSpeed = character.crouchSpeed;
        character.controller.height = character.crouchColliderHeight;
        character.controller.center = new Vector3(0f, character.crouchColliderHeight / 2f, 0f);
        grounded = character.controller.isGrounded;
        gravityValue = character.gravityValue;
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Exit()
    {
        base.Exit();

        // �ݶ��̴� ���� �ʱ�ȭ
        character.controller.height = character.normalColliderHeight;
        character.controller.center = new Vector3(0f, character.normalColliderHeight / 2f, 0f);
        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);
        character.animator.SetTrigger("move");
    }

    // �Է� ó�� �޼���
    public override void HandleInput()
    {
        base.HandleInput();

        // �ɱ� ��ư�� ���� ���
        if (crouchAction.triggered && !belowCeiling)
        {
            crouchHeld = true;
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

        // �ִϸ��̼� ��� �� ���� ��ȯ
        character.animator.SetFloat("speed", input.magnitude, character.speedDampTime, Time.deltaTime);
        if (crouchHeld)
        {
            stateMachine.ChangeState(character.standing);
        }
    }

    // ���� ������Ʈ �޼���
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // õ�� �Ʒ� �浹 üũ �� �߷� ����
        belowCeiling = CheckCollisionOverlap(character.transform.position + Vector3.up * character.normalColliderHeight);
        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = character.controller.isGrounded;
        if (grounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        // �ӵ� ���� �ε巴�� ��ȭ
        currentVelocity = Vector3.Lerp(currentVelocity, velocity, character.velocityDampTime);
        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        // ĳ���� ȸ��
        if (velocity.magnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }
    }

    // õ����� �浹 üũ�ϴ� �޼���
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
