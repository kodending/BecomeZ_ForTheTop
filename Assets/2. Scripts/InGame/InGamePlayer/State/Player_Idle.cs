using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Idle : PlayerBaseState
{
    public Player_Idle(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        //Debug.Log("³ª ¸ØÃç¼¹´Ù");
    }

    public override void OnUpdateState()
    {
        if(!playerController.m_agent.pathPending && playerController.m_agent.remainingDistance > 0)
        {
            playerController.m_stateMachine.ChangeState(PLAYERSTATE.MOVE);
        }
    }

    public override void OnFixedUpdateState()
    {
        //playerController.m_anim.SetFloat("Speed", playerController.m_agent.velocity.magnitude);
        playerController.m_pv.RPC("AnimFloatRPC", RpcTarget.All, "Speed", playerController.m_agent.velocity.magnitude);

        if(playerController.m_agent.velocity.magnitude == 0)
        {
            playerController.transform.LookAt(Vector3.zero);
        }
    }

    public override void OnExitState()
    {

    }
}
