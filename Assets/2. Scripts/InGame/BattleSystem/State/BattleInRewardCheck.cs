using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInRewardCheck : BaseState
{
    public override void OnEnterState()
    {
        //�¸� �ִϸ��̼� ����
        GameManager.gm.m_inGamePlayer.m_pv.RPC("AnimTriggerRPC", RpcTarget.All, "Victory");

        //�����͸� ������
        if(PhotonNetwork.IsMasterClient)
        {
            for(int i = GameManager.gm.m_listOrderInfo.Count - 1; i >= 0; i--)
            {
                var timeline = GameManager.gm.m_listOrderInfo[i];

                PhotonNetwork.Destroy(timeline.gameObject);
            }
        }

        //����â �߰� �ϱ�
        //�¸� UI ����
        UIManagerInGame ui = GameManager.m_curUI.GetComponent<UIManagerInGame>();

        BattleManager.bm.StartCoroutine(ShowRewardPanel(ui));
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

    IEnumerator ShowRewardPanel(UIManagerInGame ui)
    {
        ui.ShowVictory();

        yield return new WaitForSeconds(3f);

        ui.m_goRewardPanel.SetActive(true);

        foreach (var card in ui.m_listRewardCard)
        {
            card.transform.Find("Panel").GetComponent<RewardCard>().GetRandomItem();
            UIManager.ShowVerticalScale(card);
        }
    }
}
