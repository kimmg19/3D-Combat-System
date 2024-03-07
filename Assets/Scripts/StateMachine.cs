public class StateMachine
{
    public State currentState; // 현재 상태를 저장하는 변수

    // StateMachine 초기화 메서드
    public void Initialize(State startingState)
    {
        currentState = startingState; // 현재 상태를 startingState로 설정
        startingState.Enter(); // 시작 상태의 Enter 메서드 호출
    }

    // 상태 변경 메서드
    public void ChangeState(State newState)
    {
        currentState.Exit(); // 현재 상태의 Exit 메서드 호출

        currentState = newState; // 현재 상태를 newState로 업데이트
        newState.Enter(); // 새로운 상태의 Enter 메서드 호출
    }
}
