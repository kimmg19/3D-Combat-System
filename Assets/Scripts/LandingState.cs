using UnityEngine;

public class LandingState : State
{
    // �ʿ��� ������ ����
    float timePassed; // ��� �ð�
    float landingTime; // ���� �� ��� �ð�

    // LandingState ������
    public LandingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // ���� ���� �� ����Ǵ� �޼���
    public override void Enter()
    {
        base.Enter();

        // �ʱ�ȭ
        timePassed = 0f;
        character.animator.SetTrigger("land"); // ���� �ִϸ��̼� ���
        landingTime = 0.5f; // ���� �� ��� �ð� ����
    }

    // ���� ������Ʈ �޼���
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ��� �ð��� ������ ���ִ� ���·� ��ȯ
        if (timePassed > landingTime)
        {
            character.animator.SetTrigger("move"); // �̵� �ִϸ��̼� ���
            stateMachine.ChangeState(character.standing); // ���ִ� ���·� ��ȯ
        }
        timePassed += Time.deltaTime; // ��� �ð� ������Ʈ
    }
}
