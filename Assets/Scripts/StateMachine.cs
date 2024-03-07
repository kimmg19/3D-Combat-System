public class StateMachine
{
    public State currentState; // ���� ���¸� �����ϴ� ����

    // StateMachine �ʱ�ȭ �޼���
    public void Initialize(State startingState)
    {
        currentState = startingState; // ���� ���¸� startingState�� ����
        startingState.Enter(); // ���� ������ Enter �޼��� ȣ��
    }

    // ���� ���� �޼���
    public void ChangeState(State newState)
    {
        currentState.Exit(); // ���� ������ Exit �޼��� ȣ��

        currentState = newState; // ���� ���¸� newState�� ������Ʈ
        newState.Enter(); // ���ο� ������ Enter �޼��� ȣ��
    }
}
