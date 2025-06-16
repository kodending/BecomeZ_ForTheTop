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

        //포톤으로 생성한거 다 없애야됨
        //자기 캐릭터

        if (GameManager.gm.m_inGamePlayer.m_pv.IsMine)
        {
            //먼저 정보를 저장한다.
            UserDataManager.SaveUserStats(GameManager.gm.m_inGamePlayer.m_sCurStats);

            PhotonNetwork.Destroy(GameManager.gm.m_inGamePlayer.gameObject);
        }


        //에너미
        //타임라인 다 없애야됨
        if(PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("에너미 리스트 : " + GameManager.gm.m_listEnemyInfo.Count);
            PhotonNetwork.DestroyAll();
        }

        yield return new WaitForSeconds(2.5f);

        BattleManager.m_listBattleTimelineInfo.Clear();
        BattleManager.m_listBattleInfo.Clear();

        UIManager.FadeInImage(GameManager.m_curUI.GetComponent<UIManagerInGame>().m_imgDarkFade, 1f);
        UIManager.um.StartCoroutine(GameManager.ChangeScene("InGameScene", GAMESTATE.INGAME, 1f));
    }
}
