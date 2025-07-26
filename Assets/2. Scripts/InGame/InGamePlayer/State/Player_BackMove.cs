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
        //Debug.Log("나 복귀중이야");
        //playerController.m_anim.SetBool("IsAttack", false);

        //플레이어 스킬선택에 따라서 종료해주는 것도 달라야됨
        curSkillInfo = playerController.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill];
        string animName = "";

        animName = curSkillInfo.engName;

        playerController.m_pv.RPC("AnimBoolRPC", RpcTarget.All, animName, false);

        //복귀해야하는거 RPC 쏴야 부드러울듯
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
        //이때 알려주면 되겠다 끝났다고 굿

        yield return new WaitForSeconds(0.5f);

        BattleManager.CheckNextBattle();
    }
}
