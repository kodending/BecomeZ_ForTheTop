using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AttackMove : PlayerBaseState
{
    string animName = "";

    public Player_AttackMove(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        playerController.m_agent.ResetPath();

        playerController.m_pv.RPC("AnimFloatRPC", RpcTarget.All, "Speed", 0f);

        //��ų ���ÿ� ���� ���� �ִϸ��̼��� �ٲ���ߵ�
        switch ((CLASSTYPE)playerController.m_classIdx)
        {
            case CLASSTYPE.KNIGHT:
                animName = "KnightMeleeAttackMove";
                break;

            case CLASSTYPE.DARKKNIGHT:
                animName = "DarkKnightMeleeAttackMove";
                break;

            case CLASSTYPE.RANGER:
                animName = "RangerMeleeAttackMove";
                break;
        }

        playerController.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, animName);

        //�̵��ϴ°� RPC���� �ε巯���
        var newPos = playerController.m_enemySelected.transform.position + new Vector3(0, 0, -2f);
        //playerController.transform.DOMove(newPos, 1.4f).SetEase(Ease.OutQuad);
        playerController.m_pv.RPC("DoMoveRPC", RpcTarget.All, newPos, 1.4f, Ease.OutQuad);

        playerController.StartCoroutine(ChangeAttack());
    }

    public override void OnAnimatorMove()
    {

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

    IEnumerator ChangeAttack()
    {
        yield return new WaitForSeconds(1.4f);

        playerController.m_stateMachine.ChangeState(PLAYERSTATE.MELEE_ATTACk);
    }
}
