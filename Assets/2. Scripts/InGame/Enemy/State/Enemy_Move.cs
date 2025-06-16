using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : EnemyBaseState
{
    public Enemy_Move(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        enemyFSM.m_pv.RPC("DoMoveRPC", RpcTarget.All, enemyFSM.m_vMovePos, enemyFSM.m_fMoveTime, Ease.OutQuad);
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
}
