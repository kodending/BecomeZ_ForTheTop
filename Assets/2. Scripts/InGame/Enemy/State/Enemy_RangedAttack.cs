using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Enemy_RangedAttack : EnemyBaseState
{
    string animName = "";

    public Enemy_RangedAttack(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        animName = enemyFSM.m_sSelectedSkill.name;

        enemyFSM.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, animName);
    }

    public override void OnAnimatorMove()
    {

    }

    public override void OnUpdateState()
    {
        if (enemyFSM.m_anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
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
                Debug.Log("종료종료");
                // 애니메이션 종료
                enemyFSM.StartCoroutine(ChangeIdle());
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
        enemyFSM.m_stateMachine.ChangeState(ENEMYSTATE.IDLE);
        //이때 알려주면 되겠다 끝났다고 굿
        enemyFSM.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "AttackToIdle");

        yield return new WaitForSeconds(0.5f);

        BattleManager.CheckNextBattle();
    }
}
