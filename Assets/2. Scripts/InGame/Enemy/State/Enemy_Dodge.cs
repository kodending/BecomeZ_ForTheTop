using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Enemy_Dodge : EnemyBaseState
{
    public Enemy_Dodge(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        enemyFSM.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "Dodge");
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
}
