using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player_Move : PlayerBaseState
{
    public Player_Move(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        //Debug.Log("나 이동 중이다.");
    }

    public override void OnAnimatorMove()
    {

    }

    public override void OnUpdateState()
    {
        if (playerController.m_agent.remainingDistance <= playerController.m_agent.stoppingDistance)
        {
            if (!playerController.m_agent.hasPath || playerController.m_agent.velocity.sqrMagnitude == 0f)
            {
                playerController.m_agent.ResetPath();
                playerController.m_stateMachine.ChangeState(PLAYERSTATE.IDLE);
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
}
