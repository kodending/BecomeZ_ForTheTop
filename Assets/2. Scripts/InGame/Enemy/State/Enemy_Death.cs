using Photon.Pun;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Death : EnemyBaseState
{
    public Enemy_Death(EnemyFSM enemy) : base(enemy) { }

    public override void OnEnterState()
    {
        enemyFSM.StartCoroutine(ChangeRagdoll());

        //���⼭ ���� �������Ѿߵ�.
        //���������� ȣ���� �ʿ����

        for(int idx = 0; idx < BattleManager.m_listBattleTimelineInfo.Count; idx++)
        {
            var info = BattleManager.m_listBattleTimelineInfo[idx];

            if(!info.bUser)
            {
                if (info.go.GetComponent<EnemyFSM>() == enemyFSM)
                {
                    if(PhotonNetwork.IsMasterClient)
                        PhotonNetwork.Destroy(GameManager.gm.m_listOrderInfo[idx].gameObject);

                    BattleManager.m_listBattleTimelineInfo.RemoveAt(idx);
                }
            }
        }

        foreach (var info in BattleManager.m_listBattleInfo)
        {
            if (!info.bUser)
            {
                if (info.go.GetComponent<EnemyFSM>() == enemyFSM)
                {
                    BattleManager.m_listBattleInfo.Remove(info);
                    break;
                }
            }
        }

        for (int idx = 0; idx < BattleManager.m_listReadyBattleTime.Count; idx++)
        {
            var info = BattleManager.m_listReadyBattleTime[idx];

            if (!info.bUser)
            {
                if (info.go.GetComponent<EnemyFSM>() == enemyFSM)
                {
                    BattleManager.m_listReadyBattleTime.RemoveAt(idx);
                }
            }
        }

        //Debug.Log("BattleManager.m_listReadyBattleTime ī��Ʈ : " + BattleManager.m_listReadyBattleTime.Count);
        int enemyIdx = GameManager.gm.m_listEnemyInfo.IndexOf(enemyFSM);


        //����Ʈ�� ���� ���־ߵǰ�
        BattleManager.bm.m_arrSelectedObjs[enemyIdx].gameObject.SetActive(false);
        enemyFSM.m_bSelected = false;

        //�׸��� ���־��ϰ�
        enemyFSM.m_goShadow.SetActive(false);
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

    IEnumerator ChangeRagdoll()
    {
        CopyCharTransform(enemyFSM.m_originObj.transform, enemyFSM.m_ragdollObj.transform);

        enemyFSM.m_anim.StartPlayback();

        enemyFSM.m_originObj.SetActive(false);
        enemyFSM.m_ragdollObj.SetActive(true);

        int idx = Random.Range(0, enemyFSM.m_arrRigid.Length);
        enemyFSM.m_arrRigid[idx].AddForce(new Vector3(0, 200f, 300f), ForceMode.Impulse);

        //Debug.Log("���ݹ��� ���� : " + enemyFSM.m_arrRigid[idx].name);

        yield return new WaitForSeconds(1.5f);

        Rigidbody[] arrRb = enemyFSM.m_ragdollObj.GetComponentsInChildren<Rigidbody>();

        foreach (var rigid in arrRb)
        {
            rigid.useGravity = false;
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            rigid.isKinematic = true;
        }
    }


    void CopyCharTransform(Transform origin, Transform ragdoll)
    {
        for (int i = 0; i < origin.childCount; i++)
        {
            if (origin.childCount != 0)
            {
                CopyCharTransform(origin.GetChild(i), ragdoll.GetChild(i));
            }

            ragdoll.GetChild(i).localPosition = origin.GetChild(i).localPosition;
            ragdoll.GetChild(i).localRotation = origin.GetChild(i).localRotation;

            if(origin.GetChild(i).gameObject.activeSelf)
                ragdoll.GetChild(i).gameObject.gameObject.SetActive(true);
        }
    }
}
