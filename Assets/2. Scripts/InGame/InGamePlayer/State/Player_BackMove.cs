using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_BackMove : PlayerBaseState
{
    SKILLINFO curSkillInfo = new SKILLINFO();

    public Player_BackMove(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        //Debug.Log("�� �������̾�");
        //playerController.m_anim.SetBool("IsAttack", false);

        //�÷��̾� ��ų���ÿ� ���� �������ִ� �͵� �޶�ߵ�
        curSkillInfo = playerController.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill];
        string animName = "";

        animName = curSkillInfo.engName;

        playerController.m_pv.RPC("AnimBoolRPC", RpcTarget.All, animName, false);

        //�����ؾ��ϴ°� RPC ���� �ε巯���
        var newPos = BattleManager.bm.m_trPlayerPos[GameManager.gm.m_BattlePosIdx].position;
        //playerController.transform.DOMove(newPos, 1.4f).SetEase(Ease.InOutCubic);
        playerController.m_pv.RPC("DoMoveRPC", RpcTarget.All, newPos, 1.4f, Ease.InOutCubic);
        playerController.StartCoroutine(ChangeIdle());
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

    IEnumerator ChangeIdle()
    {
        yield return new WaitForSeconds(1.4f);

        playerController.m_stateMachine.ChangeState(PLAYERSTATE.IDLE);
        //�̶� �˷��ָ� �ǰڴ� �����ٰ� ��

        yield return new WaitForSeconds(0.5f);

        BattleManager.CheckNextBattle();
    }
}
