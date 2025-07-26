using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RangedAttack : PlayerBaseState
{
    SKILLINFO curSkillInfo = new SKILLINFO();
    string animName = "";

    public Player_RangedAttack(InGamePlayerController pc) : base(pc) { }
    public override void OnEnterState()
    {
        curSkillInfo = playerController.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill];

        //스킬 선택에 따라서 공격 애니메이션을 바꿔줘야됨
        animName = curSkillInfo.engName;

        playerController.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, animName);
    }

    public override void OnAnimatorMove()
    {

    }

    public override void OnUpdateState()
    {
        if (playerController.m_anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            // 원하는 애니메이션이라면 플레이 중인지 체크
            float animTime = playerController.m_anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime == 0)
            {
                // 플레이 중이 아님
                //Debug.Log("플레이중이 아님");
            }
            if (animTime > 0 && animTime < 1.0f)
            {
                // 애니메이션 플레이 중
                //Debug.Log("플레이중");
            }
            else if (animTime >= 1.0f)
            {
                Debug.Log("종료종료");
                // 애니메이션 종료
                playerController.StartCoroutine(ChangeIdle());
            }
        }
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {

    }

    IEnumerator ChangeIdle()
    {
        playerController.m_stateMachine.ChangeState(PLAYERSTATE.IDLE);
        //이때 알려주면 되겠다 끝났다고 굿
        playerController.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "AttackToIdle");

        yield return new WaitForSeconds(0.5f);

        BattleManager.CheckNextBattle();
    }
}
