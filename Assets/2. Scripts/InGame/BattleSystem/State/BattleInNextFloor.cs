using Photon.Pun;
using PlayFab.GroupsModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInNextFloor : BaseState
{
    public override void OnEnterState()
    {
        BattleManager.MoveCam(BATTLECAMTYPE.NEXTFLOOR, 2.5f);

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
            string json = JsonUtility.ToJson(GameManager.gm.m_inGamePlayer.m_sCurStats);
            GameManager.gm.m_inGamePlayer.m_pv.RPC("SaveUserInfoRPC", RpcTarget.All,
                GameManager.gm.m_BattlePosIdx, json);
        }

        //에너미에 활성화된 오브젝트를 비활성화시켜야됨
        foreach(var enemy in GameManager.gm.m_listEnemyInfo)
        {
            enemy.InActiveObj();
        }

        //에너미
        //타임라인 다 없애야됨
        if(PhotonNetwork.IsMasterClient)
            GameManager.gm.m_listOrderInfo.Clear();

        BattleManager.m_listBattleTimelineInfo.Clear();
        BattleManager.m_listBattleInfo.Clear();

        yield return new WaitForSeconds(2f);

        UIManager.FadeInImage(GameManager.m_curUI.GetComponent<UIManagerInGame>().m_imgDarkFade, 1f);
        UIManager.um.StartCoroutine(GameManager.ChangeScene("InGameScene", GAMESTATE.INGAME, 1f));
    }
}
