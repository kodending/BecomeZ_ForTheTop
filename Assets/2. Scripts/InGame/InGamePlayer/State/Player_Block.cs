using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Block : PlayerBaseState
{
    public Player_Block(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        playerController.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "Block");
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
