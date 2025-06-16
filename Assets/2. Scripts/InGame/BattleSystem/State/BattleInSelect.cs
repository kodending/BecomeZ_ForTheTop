using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInSelect : BaseState
{
    public override void OnEnterState()
    {
        //�÷��̾� �϶�
        if(BattleManager.m_listBattleTimelineInfo[0].bUser)
        {
            if (BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>().m_pv.IsMine)
            {
                //�̷��� ���� �г��� �߰� �����.
                GameManager.m_curUI.GetComponent<UIManagerInGame>().OnBattleSelectPanel();
            }

            BattleManager.MoveCam(BATTLECAMTYPE.PLAYERATTACK, 0f, 1.0f);
        }

        //�ƴҋ�
        else if(!BattleManager.m_listBattleTimelineInfo[0].bUser)
        {
            //���ʹ̰� �����ϴ� �˰����� ȣ���Ѵ�
           // Debug.Log("�� ���ʳ�");

            if (BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<EnemyFSM>().m_pv.IsMine)
            {
                GameManager.m_curUI.GetComponent<UIManagerInGame>().EnemyAttackSelect(BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<EnemyFSM>());
            }

            BattleManager.MoveCam(BATTLECAMTYPE.ENEMYATTACK, 0f, 1.0f);
        }

        //�ƹ��͵� �ƴҶ�
        else
        {
            //���� ����� �޼����� ����
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
