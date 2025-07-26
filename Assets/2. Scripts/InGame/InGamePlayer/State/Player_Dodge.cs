using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Player_Dodge : PlayerBaseState
{
    public Player_Dodge(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        playerController.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "Dodge");

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
