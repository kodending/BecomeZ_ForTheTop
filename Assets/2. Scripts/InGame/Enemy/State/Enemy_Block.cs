using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Block : EnemyBaseState
{
    public Enemy_Block(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        enemyFSM.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "Block");
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
