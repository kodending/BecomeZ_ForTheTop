using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BackMove : EnemyBaseState
{
    public Enemy_BackMove(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        enemyFSM.m_pv.RPC("AnimBoolRPC", RpcTarget.All, enemyFSM.m_sSelectedSkill.name, false);

        int idx = GameManager.gm.m_listEnemyInfo.IndexOf(enemyFSM);
        var newPos = BattleManager.bm.m_trEnemyPos[idx].position;
        enemyFSM.m_pv.RPC("DoMoveRPC", RpcTarget.All, newPos, 1.1f, Ease.InOutCubic);
        enemyFSM.StartCoroutine(ChangeIdle());
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

    IEnumerator ChangeIdle()
    {
        yield return new WaitForSeconds(1.1f);

        enemyFSM.m_stateMachine.ChangeState(ENEMYSTATE.IDLE);
        //�̶� �˷��ָ� �ǰڴ� �����ٰ� ��

        yield return new WaitForSeconds(0.5f);

        //�̰� ��������̴ϱ� �� ��Ʋ�Ŵ������� Ȯ���ϴ°� ������
        BattleManager.CheckNextBattle();
    }
}
