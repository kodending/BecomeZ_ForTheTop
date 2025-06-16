using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Dodge : PlayerBaseState
{
    public Player_Dodge(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        playerController.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "Dodge");
    }

    public override void OnUpdateState()
    {
        if (playerController.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
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

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        int idx = GameManager.gm.m_listPlayerInfo.IndexOf(playerController);

        playerController.m_vMovePos = BattleManager.bm.m_trPlayerPos[idx].position;
        playerController.m_fMoveTime = 0.7f;
    }
}
