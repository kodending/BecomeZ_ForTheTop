using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInApear : BaseState
{
    #region ���� �����Ǵ� �ۼ�Ʈ ����
    static readonly Dictionary<int, int[]> SpawnTable = new()
    {
        //1��
        { 1, new[] { 60, 40 } },

        //2��
        { 2, new[] { 30, 70 } },

        //3��
        { 3, new[] { 20, 30, 50 } },

        //4��
        { 4, new[] { 10, 20, 30, 40 } },

        //5��
        { 5, new[] { 0, 0, 20, 30, 50 } },

        //6��
        { 6, new[] { 0, 0, 5, 15, 30, 50 } },

        //7��
        { 7, new[] { 0, 0, 0, 5, 15, 30, 50 } },

        //8��
        { 8, new[] { 0, 0, 0, 0, 0, 0, 0, 100 } },

        //9��
        { 9, new[] { 0, 0, 0, 0, 0, 15, 35, 0, 50 } },

        //10��
        { 10, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 100 } },
    };

    #endregion

    int m_iSpawnNum = 0;

    public override void OnEnterState()
    {
        AudioManager.PlayBGM(BGM.BATTLE_NORMAL, true);

        //�� ����
        BattleManager.m_iCurFloor++;

        BattleManager.InitCamPos();

        BattleManager.MoveCam(BATTLECAMTYPE.PLAYERATTACK, 1.5f, DG.Tweening.Ease.InQuint);

        //�̋� ������ ��ų�� �����س���
        //���ʹ� ��ȯ�Ҷ� �������� �˾ƾ��ϰ�
        //�� ���� ���� ���͸� ���� ��ȯ�ؾߵ�
        //���ʹ� ��ȯ�ϴ°�

        var LVInfo = GoogleSheetManager.m_levelDesignInfo[BattleManager.m_iCurFloor - 1];

        int iCnt = int.Parse(LVInfo["HEADCOUNT"].ToString());

        if (PhotonNetwork.IsMasterClient) StartCoroutine(RespawnEnemy(iCnt));
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

        //���� �� �������� ���� ��ȯ�Ұ��� ���ؾߵ�
        var LVInfo = GoogleSheetManager.m_levelDesignInfo[BattleManager.m_iCurFloor - 1];

        int iRange = int.Parse(LVInfo["ENEMYRANGE"].ToString()) - 1;
        int randIdx = GetRandomMonserIndex(BattleManager.m_iCurFloor, iRange) + 1;

        //�׽�Ʈ �뵵
        //randIdx = 10;
        // ** //rm 

        Debug.Log("iMaxIndex : " + iRange + "randIdx : " + randIdx);

        var enemy = PhotonNetwork.Instantiate("ENEMIES_Enemy", pos.position, Quaternion.Euler(0, 180, 0));
        enemy.GetComponent<EnemyFSM>().m_pv.RPC("InitEnemyInfo", RpcTarget.All, randIdx);
        m_iSpawnNum++;

        if (m_iSpawnNum >= maxSpawn) yield break;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(RespawnEnemy(maxSpawn));
    }

    int SpawnEnemyIndex(int iRange)
    {
        int idx = 0;
        int[] arrEnemyIdx = new int[100];

        //�ش� �� ���� �ε����� ����� Ȯ�� 50%
        for(int i = 0; i < 100; i++)
        {
            if(i < 51)
            {
                arrEnemyIdx[i] = iRange;
            }

            else
            {
                int random = Random.Range(1, iRange);
                arrEnemyIdx[i] = random;
            }
        }

        int randNum = Random.Range(0, 100);
        idx = arrEnemyIdx[randNum];

        return idx;
    }

    int GetRandomMonserIndex(int curFloor, int maxIndex)
    {
        if (!SpawnTable.TryGetValue(curFloor, out var weights))
            weights = new[] { 100 };

        if (weights.Length > maxIndex + 1)
            weights = weights[..(maxIndex + 1)];

        int total = 0;
        foreach (int w in weights) total += w;

        int pick = Random.Range(0, total);      
        for (int i = 0; i < weights.Length; i++)
        {
            pick -= weights[i];
            if (pick < 0) return i;             
        }
        return 0;                              
    }
}
