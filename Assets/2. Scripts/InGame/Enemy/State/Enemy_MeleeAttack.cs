using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_MeleeAttack : EnemyBaseState
{
    string animName = "";

    public Enemy_MeleeAttack(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        animName = enemyFSM.m_sSelectedSkill.name;

        enemyFSM.m_pv.RPC("AnimBoolRPC", RpcTarget.All, animName, true);
    }

    public override void OnAnimatorMove()
    {

    }

    public override void OnUpdateState()
    {
        if (enemyFSM.m_anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
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
                enemyFSM.m_stateMachine.ChangeState(ENEMYSTATE.BACK_MOVE);
            }
        }
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {

    }
}
