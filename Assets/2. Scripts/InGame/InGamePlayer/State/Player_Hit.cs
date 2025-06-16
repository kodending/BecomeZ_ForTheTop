using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Hit : PlayerBaseState
{
    public Player_Hit(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        playerController.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "Hit");
    }

    public override void OnUpdateState()
    {
        if (playerController.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            // ���ϴ� �ִϸ��̼��̶�� �÷��� ������ üũ
            float animTime = playerController.m_anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime == 0)
            {
                // �÷��� ���� �ƴ�
                //Debug.Log("�÷������� �ƴ�");
            }
            if (animTime > 0 && animTime < 1.0f)
            {
                // �ִϸ��̼� �÷��� ��
                //Debug.Log("�÷�����");
            }
            else if (animTime >= 1.0f)
            {
                //Debug.Log("��������");
                // �ִϸ��̼� ����
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
