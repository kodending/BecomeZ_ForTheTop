using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AttackMove : PlayerBaseState
{
    public Player_AttackMove(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        //Debug.Log("나 전투이동중");
        //playerController.m_anim.SetFloat("Speed", 0f);
        //playerController.m_anim.SetTrigger("AttackMove");
        playerController.m_agent.ResetPath();

        playerController.m_pv.RPC("AnimFloatRPC", RpcTarget.All, "Speed", 0f);
        playerController.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "AttackMove");

        //이동하는거 RPC쏴야 부드러울듯
        var newPos = playerController.m_enemySelected.transform.position + new Vector3(0, 0, -2f);
        //playerController.transform.DOMove(newPos, 1.4f).SetEase(Ease.OutQuad);
        playerController.m_pv.RPC("DoMoveRPC", RpcTarget.All, newPos, 1.4f, Ease.OutQuad);

        playerController.StartCoroutine(ChangeAttack());
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

        playerController.m_stateMachine.ChangeState(PLAYERSTATE.ATTACK);
    }
}
