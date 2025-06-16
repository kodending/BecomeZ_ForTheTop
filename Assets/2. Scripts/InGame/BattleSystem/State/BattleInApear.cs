using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInApear : BaseState
{
    int m_iSpawnNum = 0;

    public override void OnEnterState()
    {
        BattleManager.InitCamPos();

        BattleManager.MoveCam(BATTLECAMTYPE.PLAYERATTACK, 0.5f, 2.5f);

        //이떄 본인의 스킬을 세팅해놓기

        //에너미 소환하는곳
        if(PhotonNetwork.IsMasterClient) StartCoroutine(RespawnEnemy(1));
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {
        m_iSpawnNum = 0;
    }

    IEnumerator RespawnEnemy(int maxSpawn)
    {
        var pos = BattleManager.bm.m_trEnemyPos[m_iSpawnNum];
        var enemy = PhotonNetwork.Instantiate("ENEMIES_Enemy", pos.position, Quaternion.Euler(0, 180, 0));
        enemy.GetComponent<EnemyFSM>().m_pv.RPC("InitEnemyInfo", RpcTarget.All, 1, 4f);
        m_iSpawnNum++;

        if (m_iSpawnNum >= maxSpawn) yield break;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(RespawnEnemy(maxSpawn));
    }
}
