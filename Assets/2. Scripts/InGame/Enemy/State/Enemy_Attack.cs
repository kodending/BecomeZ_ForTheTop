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
            // 원하는 애니메이션이라면 플레이 중인지 체크
            float animTime = enemyFSM.m_anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
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
                //Debug.Log("종료종료");
                // 애니메이션 종료
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
        //이미 데미지 정보는 정해져있다.


        //우선 물리공격, 마법공격 구분해야하고
        //공격데미지 계산하고
        //공격 데미지 - 방어력 방어력이 높으면 방어시키고
        //그게아니면 미스나는 확률 계산해서 미스띄우고
        //그게아니면 공격유효타가 들어가게한다.

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
