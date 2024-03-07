using UnityEngine;

public class SprintJumpState : State
{
    // 필요한 변수들 선언
    float timePassed; // 경과 시간
    float jumpTime; // 점프 후 대기 시간

    // SprintJumpState 생성자
    public SprintJumpState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // 상태 진입 시 실행되는 메서드
    public override void Enter()
    {
        base.Enter();

        // 초기화
        character.animator.applyRootMotion = true; // 루트 모션 적용
        timePassed = 0f;
        character.animator.SetTrigger("sprintJump"); // 스프린트 점프 애니메이션 재생

        jumpTime = 1f; // 점프 후 대기 시간 설정
    }

    // 상태 종료 시 실행되는 메서드
    public override void Exit()
    {
        base.Exit();

        // 루트 모션 적용 해제
        character.animator.applyRootMotion = false;
    }

    // 로직 업데이트 메서드
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 대기 시간이 지나면 스프린트 상태로 전환
        if (timePassed > jumpTime)
        {
            character.animator.SetTrigger("move"); // 이동 애니메이션 재생
            stateMachine.ChangeState(character.sprinting); // 스프린트 상태로 전환
        }
        timePassed += Time.deltaTime; // 경과 시간 업데이트
    }
}
