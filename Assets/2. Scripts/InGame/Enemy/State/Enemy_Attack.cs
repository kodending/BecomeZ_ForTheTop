using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : EnemyBaseState
{
    public Enemy_Attack(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        enemyFSM.m_pv.RPC("AnimBoolRPC", RpcTarget.All, "IsAttack", true);

        enemyFSM.StartCoroutine(AttackDamage(1.2f));
    }

    public override void OnUpdateState()
    {
        if (enemyFSM.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            // ���ϴ� �ִϸ��̼��̶�� �÷��� ������ üũ
            float animTime = enemyFSM.m_anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
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
                //Debug.Log("��������");
                // �ִϸ��̼� ����
                enemyFSM.m_stateMachine.ChangeState(ENEMYSTATE.BACK_MOVE);
            }
        }
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {

    }

    IEnumerator AttackDamage(float fTime)
    {
        yield return new WaitForSeconds(fTime - 0.2f);
        //�̹� ������ ������ �������ִ�.


        //�켱 ��������, �������� �����ؾ��ϰ�
        //���ݵ����� ����ϰ�
        //���� ������ - ���� ������ ������ ����Ű��
        //�װԾƴϸ� �̽����� Ȯ�� ����ؼ� �̽�����
        //�װԾƴϸ� ������ȿŸ�� �����Ѵ�.

        bool isCritical = CriticalCal();

        int atkPower = (enemyFSM.m_sCurStats.Physical_POW * 5);

        int damage = AttackWeight.AttackDamageCal(enemyFSM.m_listAtkSuccess, atkPower, isCritical);

        damage = damage - enemyFSM.m_selectedPlayer.m_sCurStats.Physical_DEF;
        //damage = damage - damage;

        int idx = GameManager.gm.m_listPlayerInfo.IndexOf(enemyFSM.m_selectedPlayer);

        ATTACKRESULT eResult = ATTACKRESULT.PHYSICAL_ATTACK;

        if(damage <= 0)
        {
            enemyFSM.m_selectedPlayer.m_pv.RPC("AttackBlockRPC", RpcTarget.All, idx);
            eResult = ATTACKRESULT.BLOCK;
        }

        else 
        {
            bool bMiss = AttackWeight.MissCal(enemyFSM.m_listAtkSuccess, enemyFSM.m_sCurStats.SPD, (float)enemyFSM.m_selectedPlayer.m_sCurStats.SPD);

            if(bMiss)
            {
                enemyFSM.m_selectedPlayer.m_pv.RPC("AttackDodgeRPC", RpcTarget.All, idx);
                eResult = ATTACKRESULT.MISS;
            }

            else
            {
                enemyFSM.m_selectedPlayer.m_pv.RPC("AttackHitRPC", RpcTarget.All, idx, damage, isCritical);
            }
        }

        NetworkManager.nm.PV.RPC("EnemyAttackDamageUI", RpcTarget.All, damage, idx, (int)eResult);
    }

    bool CriticalCal()
    {
        foreach (var suc in enemyFSM.m_listAtkSuccess)
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
