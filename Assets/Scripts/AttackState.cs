using UnityEngine;

public class AttackState : State
{
    // ���� ���¸� �����ϴ� Ŭ����
    float timePassed; // ���� ���� �ð�
    float clipLength; // ���� �ִϸ��̼� Ŭ���� ����
    float clipSpeed; // ���� �ִϸ��̼� Ŭ���� ��� �ӵ�
    bool attack; // ���� ���θ� ��Ÿ���� �÷���

    // AttackState ������
    public AttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Enter()
    {
        base.Enter();

        // �ʱ�ȭ
        attack = false;
        character.animator.applyRootMotion = true;
        timePassed = 0f;

        // ���� Ʈ���� ���� �� �̵� �ӵ� ����
        character.animator.SetTrigger("attack");
        character.animator.SetFloat("speed", 0f);
    }

    // �Է� ó�� �޼���
    public override void HandleInput()
    {
        base.HandleInput();

        // ���� �Է� ó��
        if (attackAction.triggered)
        {
            attack = true;
        }
    }

    // ���� ������Ʈ �޼���
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // �ð� ��� ������Ʈ
        timePassed += Time.deltaTime;

        // ���� �ִϸ��̼� Ŭ���� ���� ����
        clipLength = character.animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
        clipSpeed = character.animator.GetCurrentAnimatorStateInfo(1).speed;

        // ������ �Ϸ�ǰ� ���� ���·� ��ȯ
        if (timePassed >= clipLength / clipSpeed && attack)
        {
            stateMachine.ChangeState(character.attacking);
        }

        // ������ �Ϸ�ǰ� ���� ���·� ��ȯ
        if (timePassed >= clipLength / clipSpeed)
        {
            stateMachine.ChangeState(character.combatting);
            character.animator.SetTrigger("move");
        }
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Exit()
    {
        base.Exit();

        // Root motion ���� ����
        character.animator.applyRootMotion = false;
    }
}
