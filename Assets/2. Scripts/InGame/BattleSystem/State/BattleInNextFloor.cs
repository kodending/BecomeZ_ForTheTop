using Photon.Pun;
using PlayFab.GroupsModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInNextFloor : BaseState
{
    public override void OnEnterState()
    {
        BattleManager.MoveCam(BATTLECAMTYPE.NEXTFLOOR, 1f, 2.0f);

        BattleManager.bm.StartCoroutine(ChangeNextFloor());
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

    IEnumerator ChangeNextFloor()
    {
        yield return new WaitForSeconds(1.0f);

        //�������� �����Ѱ� �� ���־ߵ�
        //�ڱ� ĳ����

        if (GameManager.gm.m_inGamePlayer.m_pv.IsMine)
        {
            //���� ������ �����Ѵ�.
            UserDataManager.SaveUserStats(GameManager.gm.m_inGamePlayer.m_sCurStats);

            PhotonNetwork.Destroy(GameManager.gm.m_inGamePlayer.gameObject);
        }


        //���ʹ�
        //Ÿ�Ӷ��� �� ���־ߵ�
        if(PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("���ʹ� ����Ʈ : " + GameManager.gm.m_listEnemyInfo.Count);
            PhotonNetwork.DestroyAll();
        }

        yield return new WaitForSeconds(2.5f);

        BattleManager.m_listBattleTimelineInfo.Clear();
        BattleManager.m_listBattleInfo.Clear();

        UIManager.FadeInImage(GameManager.m_curUI.GetComponent<UIManagerInGame>().m_imgDarkFade, 1f);
        UIManager.um.StartCoroutine(GameManager.ChangeScene("InGameScene", GAMESTATE.INGAME, 1f));
    }
}
