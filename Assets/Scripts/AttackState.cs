using UnityEngine;

public class AttackState : State
{
    // 공격 상태를 관리하는 클래스
    float timePassed; // 공격 진행 시간
    float clipLength; // 현재 애니메이션 클립의 길이
    float clipSpeed; // 현재 애니메이션 클립의 재생 속도
    bool attack; // 공격 여부를 나타내는 플래그

    // AttackState 생성자
    public AttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    // 상태 진입 시 실행되는 메서드
    public override void Enter()
    {
        base.Enter();

        // 초기화
        attack = false;
        character.animator.applyRootMotion = true;
        timePassed = 0f;

        // 공격 트리거 설정 및 이동 속도 설정
        character.animator.SetTrigger("attack");
        character.animator.SetFloat("speed", 0f);
    }

    // 입력 처리 메서드
    public override void HandleInput()
    {
        base.HandleInput();

        // 공격 입력 처리
        if (attackAction.triggered)
        {
            attack = true;
        }
    }

    // 로직 업데이트 메서드
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 시간 경과 업데이트
        timePassed += Time.deltaTime;

        // 현재 애니메이션 클립의 정보 갱신
        clipLength = character.animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
        clipSpeed = character.animator.GetCurrentAnimatorStateInfo(1).speed;

        // 공격이 완료되고 공격 상태로 전환
        if (timePassed >= clipLength / clipSpeed && attack)
        {
            stateMachine.ChangeState(character.attacking);
        }

        // 공격이 완료되고 전투 상태로 전환
        if (timePassed >= clipLength / clipSpeed)
        {
            stateMachine.ChangeState(character.combatting);
            character.animator.SetTrigger("move");
        }
    }

    // 상태 종료 시 실행되는 메서드
    public override void Exit()
    {
        base.Exit();

        // Root motion 적용 해제
        character.animator.applyRootMotion = false;
    }
}
