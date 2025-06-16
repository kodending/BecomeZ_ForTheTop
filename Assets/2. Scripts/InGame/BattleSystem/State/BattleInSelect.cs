using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInSelect : BaseState
{
    public override void OnEnterState()
    {
        //플레이어 일때
        if(BattleManager.m_listBattleTimelineInfo[0].bUser)
        {
            if (BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>().m_pv.IsMine)
            {
                //이럴땐 선택 패널을 뜨게 만든다.
                GameManager.m_curUI.GetComponent<UIManagerInGame>().OnBattleSelectPanel();
            }

            BattleManager.MoveCam(BATTLECAMTYPE.PLAYERATTACK, 0f, 1.0f);
        }

        //아닐떄
        else if(!BattleManager.m_listBattleTimelineInfo[0].bUser)
        {
            //에너미가 선택하는 알고리즘을 호출한다
           // Debug.Log("내 차례네");

            if (BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<EnemyFSM>().m_pv.IsMine)
            {
                GameManager.m_curUI.GetComponent<UIManagerInGame>().EnemyAttackSelect(BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<EnemyFSM>());
            }

            BattleManager.MoveCam(BATTLECAMTYPE.ENEMYATTACK, 0f, 1.0f);
        }

        //아무것도 아닐때
        else
        {
            //선택 대기중 메세지를 띄운다
        }
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
}
