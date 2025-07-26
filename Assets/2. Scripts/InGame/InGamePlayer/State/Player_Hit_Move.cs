using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Hit_Move : PlayerBaseState
{
    [Range(0f, 2f)] public float speedMultiplier = 1f;

    public Player_Hit_Move(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        playerController.m_agent.enabled = false;

        playerController.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "Hit_Move");
    }

    public override void OnUpdateState()
    {
        if (playerController.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Hit_Move"))
        {
            // 원하는 애니메이션이라면 플레이 중인지 체크
            float animTime = playerController.m_anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime == 0)
            {
                // 플레이 중이 아님
                //Debug.Log("플레이중이 아님");
            }
            if (animTime > 0 && animTime < 1.0f)
            {
                // 애니메이션 플레이 중
                //Debug.Log("플레이중");
            }
            else if (animTime >= 1.0f)
            {
                //Debug.Log("종료종료");
                // 애니메이션 종료
                playerController.m_stateMachine.ChangeState(PLAYERSTATE.RETURN_MOVE);
            }
        }
    }
    public override void OnAnimatorMove()
    {
        Vector3 delta = playerController.m_anim.deltaPosition * speedMultiplier;

        delta.y = 0f;

        var rb = playerController.GetComponent<Rigidbody>();

        rb.MovePosition(rb.position + delta);
    }
    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        //int idx = GameManager.gm.m_listPlayerInfo.IndexOf(playerController);

        playerController.m_vMovePos = BattleManager.bm.m_trPlayerPos[GameManager.gm.m_BattlePosIdx].position;
        playerController.m_fMoveTime = 0.7f;

        playerController.m_agent.enabled = true;
    }
}
