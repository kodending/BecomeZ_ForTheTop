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
        //어택 타이밍 수치가 들어가야됨
        //나이트 기본공격 타이밍 0.5
        //다크나이트 확인중 
        float HitTime = playerController.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill].fHitTime;
        playerController.StartCoroutine(AttackDamage(HitTime));
    }

    public override void OnUpdateState()
    {
        if (playerController.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
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

    IEnumerator AttackDamage(float fTime)
    {
        yield return new WaitForSeconds(fTime - 0.2f);
        //이미 데미지 정보는 정해져있다.
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
