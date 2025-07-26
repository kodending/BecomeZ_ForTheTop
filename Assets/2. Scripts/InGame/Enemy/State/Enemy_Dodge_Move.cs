using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dodge_Move : EnemyBaseState
{
    [Range(0f, 2f)] public float speedMultiplier = 1f;

    public Enemy_Dodge_Move(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        enemyFSM.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "Dodge_Move");
    }

    public override void OnUpdateState()
    {
        if (enemyFSM.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Dodge_Move"))
        {
            // ���ϴ� �ִϸ��̼��̶�� �÷��� ������ üũ
            float animTime = enemyFSM.m_anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
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
                enemyFSM.m_stateMachine.ChangeState(ENEMYSTATE.MOVE);
            }
        }
    }

    public override void OnAnimatorMove()
    {
        Vector3 delta = enemyFSM.m_anim.deltaPosition * speedMultiplier;

        delta.y = 0f;

        var rb = enemyFSM.GetComponent<Rigidbody>();

        rb.MovePosition(rb.position + delta);
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        int idx = GameManager.gm.m_listEnemyInfo.IndexOf(enemyFSM);

        enemyFSM.m_vMovePos = BattleManager.bm.m_trEnemyPos[idx].position;
        enemyFSM.m_fMoveTime = 0.7f;
    }
}
