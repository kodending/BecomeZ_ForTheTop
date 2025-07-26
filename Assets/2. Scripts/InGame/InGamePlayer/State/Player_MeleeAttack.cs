using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Player_MeleeAttack : PlayerBaseState
{
    SKILLINFO curSkillInfo = new SKILLINFO();
    string animName = "";

    public Player_MeleeAttack(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        //선택한 나의 스킬정보를 받는다.
        curSkillInfo = playerController.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill];

        //playerController.m_anim.SetBool("IsAttack", true);
        //스킬 선택에 따라서 공격 애니메이션을 바꿔줘야됨
        animName = curSkillInfo.engName;

        playerController.m_pv.RPC("AnimBoolRPC", RpcTarget.All, animName, true);
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
                playerController.m_stateMachine.ChangeState(PLAYERSTATE.BACK_MOVE);
            }
        }
    }

    public override void OnFixedUpdateState()
    {
        //playerController.m_anim.SetFloat("Speed", playerController.m_agent.velocity.magnitude);
        playerController.m_pv.RPC("AnimFloatRPC", RpcTarget.All, "Speed", playerController.m_agent.velocity.magnitude);
    }

    public override void OnExitState()
    {
        
    }
}
