using UnityEngine;

public class LandingState : State
{
    // 필요한 변수들 선언
    float timePassed; // 경과 시간
    float landingTime; // 착지 후 대기 시간

    // LandingState 생성자
    public LandingState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // 상태 진입 시 실행되는 메서드
    public override void Enter()
    {
        base.Enter();

        // 초기화
        timePassed = 0f;
        character.animator.SetTrigger("land"); // 착지 애니메이션 재생
        landingTime = 0.5f; // 착지 후 대기 시간 설정
    }

    // 로직 업데이트 메서드
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 대기 시간이 지나면 서있는 상태로 전환
        if (timePassed > landingTime)
        {
            character.animator.SetTrigger("move"); // 이동 애니메이션 재생
            stateMachine.ChangeState(character.standing); // 서있는 상태로 전환
        }
        timePassed += Time.deltaTime; // 경과 시간 업데이트
    }
}
