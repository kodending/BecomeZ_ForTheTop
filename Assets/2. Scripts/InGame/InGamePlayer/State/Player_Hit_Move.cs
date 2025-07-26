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
