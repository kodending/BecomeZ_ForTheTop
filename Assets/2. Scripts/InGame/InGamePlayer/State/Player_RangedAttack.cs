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

        //��ų ���ÿ� ���� ���� �ִϸ��̼��� �ٲ���ߵ�
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
        //�̶� �˷��ָ� �ǰڴ� �����ٰ� ��
        playerController.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "AttackToIdle");

        yield return new WaitForSeconds(0.5f);

        BattleManager.CheckNextBattle();
    }
}
