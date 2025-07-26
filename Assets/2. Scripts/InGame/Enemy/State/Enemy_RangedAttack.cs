using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Enemy_RangedAttack : EnemyBaseState
{
    string animName = "";

    public Enemy_RangedAttack(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        animName = enemyFSM.m_sSelectedSkill.name;

        enemyFSM.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, animName);
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
                Debug.Log("��������");
                // �ִϸ��̼� ����
                enemyFSM.StartCoroutine(ChangeIdle());
            }
        }
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {

    }

    IEnumerator ChangeIdle()
    {
        enemyFSM.m_stateMachine.ChangeState(ENEMYSTATE.IDLE);
        //�̶� �˷��ָ� �ǰڴ� �����ٰ� ��
        enemyFSM.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "AttackToIdle");

        yield return new WaitForSeconds(0.5f);

        BattleManager.CheckNextBattle();
    }
}
