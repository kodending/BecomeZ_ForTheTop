using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInGame : BaseState
{
    public override void OnEnterState()
    {
        GameManager.m_curState = GAMESTATE.INGAME;
        GameManager.m_curUI = GameObject.Find("UIManagerInGame");

        Vector3 pos = BattleManager.bm.m_trInitPlayerPos[GameManager.gm.m_BattlePosIdx].position;

        GameObject inGamePlayer = PhotonNetwork.Instantiate("INGAMEPLAYER_1", pos, Quaternion.identity);
        inGamePlayer.GetComponent<InGamePlayerController>().InitInfo();

        //지금 나온 인게임플레이어를 배틀정보리스트에 넣어야한다. 다 전파해야한다.
        int classIdx = inGamePlayer.GetComponent<InGamePlayerController>().m_classIdx;
        inGamePlayer.GetComponent<InGamePlayerController>().m_pv.RPC("AddBattleListRPC", RpcTarget.Others, classIdx);
        inGamePlayer.SetActive(true);
    }

    public override void OnUpdateState()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (GameManager.gm.m_listEnemyInfo.Count == 0) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            { 
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    int idx = 0;
                    for(; idx < GameManager.gm.m_listEnemyInfo.Count; idx++)
                    {
                        var enemy = GameManager.gm.m_listEnemyInfo[idx];
                        if(enemy == hit.transform.GetComponent<EnemyFSM>())
                        {
                            BattleManager.SelectEnemy(idx, enemy.gameObject);

                            break;
                        }
                    }
                }
            }
        }
    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {

    }
}
