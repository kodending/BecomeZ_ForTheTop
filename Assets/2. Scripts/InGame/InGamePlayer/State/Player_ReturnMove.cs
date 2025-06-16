using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ReturnMove : PlayerBaseState
{
    public Player_ReturnMove(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        playerController.m_pv.RPC("DoMoveRPC", RpcTarget.All, playerController.m_vMovePos, playerController.m_fMoveTime, Ease.OutQuad);
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
