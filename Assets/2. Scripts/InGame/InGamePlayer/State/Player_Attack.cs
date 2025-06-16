using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : PlayerBaseState
{
    public Player_Attack(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        //playerController.m_anim.SetBool("IsAttack", true);
        playerController.m_pv.RPC("AnimBoolRPC", RpcTarget.All, "IsAttack", true);
        //���� Ÿ�̹� ��ġ�� ���ߵ�
        //����Ʈ �⺻���� Ÿ�̹� 0.5
        //��ũ����Ʈ Ȯ���� 
        float HitTime = playerController.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill].fHitTime;
        playerController.StartCoroutine(AttackDamage(HitTime));
    }

    public override void OnUpdateState()
    {
        if (playerController.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
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

    IEnumerator AttackDamage(float fTime)
    {
        yield return new WaitForSeconds(fTime - 0.2f);
        //�̹� ������ ������ �������ִ�.
        bool isCritical = CriticalCal();

        //int atkPower = (playerController.m_sCurStats.POW * 3);
        int atkPower = (playerController.m_sCurStats.Physical_POW * 5);

        int damage = AttackWeight.AttackDamageCal(playerController.m_listAtkSuccess, atkPower, isCritical);

        damage = damage - playerController.m_enemySelected.m_sCurStats.Physical_DEF;
        //damage = damage - damage;

        int idx = GameManager.gm.m_listEnemyInfo.IndexOf(playerController.m_enemySelected);

        ATTACKRESULT eResult = ATTACKRESULT.PHYSICAL_ATTACK;

        if (damage <= 0)
        {
            playerController.m_enemySelected.m_pv.RPC("AttackBlockRPC", RpcTarget.All, idx);
            eResult = ATTACKRESULT.BLOCK;
        }

        else
        {
            bool bMiss = AttackWeight.MissCal(playerController.m_listAtkSuccess, (float)playerController.m_sCurStats.SPD, playerController.m_enemySelected.m_sCurStats.SPD);

            if (bMiss)
            {
                playerController.m_enemySelected.m_pv.RPC("AttackDodgeRPC", RpcTarget.All, idx);
                eResult = ATTACKRESULT.MISS;
            }

            else
            {
                playerController.m_enemySelected.m_pv.RPC("AttackHitRPC", RpcTarget.All, idx, damage, isCritical);
            }
        }

        NetworkManager.nm.PV.RPC("UserAttackDamageUI", RpcTarget.All, damage, idx, (int)eResult);
    }

    bool CriticalCal()
    {
        foreach (var suc in playerController.m_listAtkSuccess)
        {
            if (!suc)
            {
                return false;
            }
        }

        bool bCri = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));

        return bCri;
    }
}
