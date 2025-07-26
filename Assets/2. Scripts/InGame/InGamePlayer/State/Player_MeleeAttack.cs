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
        //������ ���� ��ų������ �޴´�.
        curSkillInfo = playerController.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill];

        //playerController.m_anim.SetBool("IsAttack", true);
        //��ų ���ÿ� ���� ���� �ִϸ��̼��� �ٲ���ߵ�
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
            // ���ϴ� �ִϸ��̼��̶�� �÷��� ������ üũ
            float animTime = playerController.m_anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime == 0)
            {
                // �÷��� ���� �ƴ�
                //Debug.Log("�÷������� �ƴ�");
            }
            if (animTime > 0 && animTime < 1.0f)
            {
                // �ִϸ��̼� �÷��� ��
                //Debug.Log("�÷�����");
            }
            else if (animTime >= 1.0f) 
            {
                Debug.Log("��������");
                // �ִϸ��̼� ����
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
