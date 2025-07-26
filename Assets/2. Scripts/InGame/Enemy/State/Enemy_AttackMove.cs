using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackMove : EnemyBaseState
{
    public Enemy_AttackMove(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        //Debug.Log("나 전투이동중");
        enemyFSM.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "AttackMove");

        
        var newPos = enemyFSM.m_selectedPlayer.transform.position + new Vector3(0, 0, +1.7f);
        enemyFSM.m_pv.RPC("DoMoveRPC", RpcTarget.All, newPos, 1.4f, Ease.OutQuad);
        enemyFSM.StartCoroutine(ChangeAttack());
    }

    public override void OnAnimatorMove()
    {

    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {

    }

    IEnumerator ChangeAttack()
    {
        yield return new WaitForSeconds(1.4f);

        enemyFSM.m_stateMachine.ChangeState(ENEMYSTATE.MELEE_ATTACK);
    }
}
