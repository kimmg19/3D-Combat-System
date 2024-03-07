using UnityEngine;

public class SprintJumpState : State
{
    // �ʿ��� ������ ����
    float timePassed; // ��� �ð�
    float jumpTime; // ���� �� ��� �ð�

    // SprintJumpState ������
    public SprintJumpState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Enter()
    {
        base.Enter();

        // �ʱ�ȭ
        character.animator.applyRootMotion = true; // ��Ʈ ��� ����
        timePassed = 0f;
        character.animator.SetTrigger("sprintJump"); // ������Ʈ ���� �ִϸ��̼� ���

        jumpTime = 1f; // ���� �� ��� �ð� ����
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Exit()
    {
        base.Exit();

        // ��Ʈ ��� ���� ����
        character.animator.applyRootMotion = false;
    }

    // ���� ������Ʈ �޼���
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ��� �ð��� ������ ������Ʈ ���·� ��ȯ
        if (timePassed > jumpTime)
        {
            character.animator.SetTrigger("move"); // �̵� �ִϸ��̼� ���
            stateMachine.ChangeState(character.sprinting); // ������Ʈ ���·� ��ȯ
        }
        timePassed += Time.deltaTime; // ��� �ð� ������Ʈ
    }
}
