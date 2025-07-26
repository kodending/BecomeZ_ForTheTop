using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Death : PlayerBaseState
{
    public Player_Death(InGamePlayerController pc) : base(pc) { }

    public override void OnEnterState()
    {
        playerController.StartCoroutine(ChangeRagdoll());

        //여기서 나를 삭제시켜야됨.
        //다음순서는 호출할 필요없음

        for (int idx = 0; idx < BattleManager.m_listBattleTimelineInfo.Count; idx++)
        {
            var info = BattleManager.m_listBattleTimelineInfo[idx];

            if (info.bUser)
            {
                if (info.go.GetComponent<InGamePlayerController>() == playerController)
                {
                    if (PhotonNetwork.IsMasterClient)
                        PhotonNetwork.Destroy(GameManager.gm.m_listOrderInfo[idx].gameObject);

                    BattleManager.m_listBattleTimelineInfo.RemoveAt(idx);
                    BattleManager.m_listReadyBattleTime.Remove(info);
                }
            }
        }

        foreach (var info in BattleManager.m_listBattleInfo)
        {
            if (info.bUser)
            {
                if (info.go.GetComponent<InGamePlayerController>() == playerController)
                {
                    BattleManager.m_listBattleInfo.Remove(info);
                    break;
                }
            }
        }

        //그림자 없애야하고
        playerController.m_goShadow.SetActive(false);

        //마인 이미지 없애야됨
        playerController.m_playerCanvas.m_imgMine.gameObject.SetActive(false);
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
        CopyCharTransform(playerController.m_originObj.transform, playerController.m_ragdollObj.transform);

        playerController.m_anim.StartPlayback();

        playerController.m_originObj.SetActive(false);
        playerController.m_ragdollObj.SetActive(true);

        int idx = Random.Range(0, playerController.m_arrRigid.Length);
        playerController.m_arrRigid[idx].AddForce(new Vector3(0, 200f, -300f), ForceMode.Impulse);

        yield return new WaitForSeconds(1.5f);

        Rigidbody[] arrRb = playerController.m_ragdollObj.GetComponentsInChildren<Rigidbody>();

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

            if (origin.GetChild(i).gameObject.activeSelf)
                ragdoll.GetChild(i).gameObject.gameObject.SetActive(true);
        }
    }
}
