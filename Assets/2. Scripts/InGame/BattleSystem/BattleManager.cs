using DG.Tweening;
using Photon.Pun;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager bm;

    [HideInInspector]
    public BMStateMachine m_stateMachine { get; private set; }

    public static BATTLESTATE m_curState;

    public Transform[] m_trInitPlayerPos;
    public Transform[] m_trPlayerPos;
    public Transform[] m_trEnemyPos;
    public GameObject[] m_arrSelectedObjs;

    public BattleCamMoving m_followMoving;
    public BattleCamMoving m_lookAtMoving;
    [SerializeField] Transform m_trInitFollowPos;
    static public List<BATTLEINFO> m_listBattleInfo = new List<BATTLEINFO>();
    static public List<BATTLEINFO> m_listBattleTimelineInfo = new List<BATTLEINFO>();
    static public List<BATTLEINFO> m_listReadyBattleTime = new();

    public List<Color> m_listUserColors;
    public List<Color> m_listAttributeColors;
    public List<Sprite> m_listAttributeSprites;

    [SerializeField] CinemachineShake m_goVirCam;

    static public int m_iCurSelectedSkill = 0;

    //���� ������ �ִ��� Ȯ���ϱ� ����
    static public int m_iCurFloor = 0;

    public CinemachineMoving cinemachineMoving;
    [SerializeField] Transform[] m_trFollow;
    [SerializeField] Transform[] m_trLookAt;
    [SerializeField] Transform m_trInitFollow;
    [SerializeField] Transform m_trInitLookAt;

    static public bool m_bSelectedAll = false;

    private void Awake()
    {
        bm = this;

        InitStateMachine();
    }

    private void Update()
    {
        ////�׽�Ʈ �뵵
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    if (Input.GetKeyDown(KeyCode.T))
        //    {
        //        KillEnemy(m_listBattleInfo[2].go);
        //    }
        //}

        m_stateMachine?.UpdateState();
    }

    private void FixedUpdate()
    {
        m_stateMachine.FixedUpdateState();
    }

    void InitStateMachine()
    {
        m_stateMachine = new BMStateMachine(BATTLESTATE.APEAR, gameObject.AddComponent<BattleInApear>());
        m_stateMachine.AddState(BATTLESTATE.SELECT, gameObject.AddComponent<BattleInSelect>());
        m_stateMachine.AddState(BATTLESTATE.BATTLE, gameObject.AddComponent<BattleInBattle>());
        m_stateMachine.AddState(BATTLESTATE.NEXTFLOOR, gameObject.AddComponent<BattleInNextFloor>());
        m_stateMachine.AddState(BATTLESTATE.REWARDCHECK, gameObject.AddComponent<BattleInRewardCheck>());

        m_stateMachine.currentState?.OnEnterState();
    }

    static public void InitCamPos()
    {
        bm.m_followMoving.transform.position = bm.m_trInitFollowPos.position;
    }

    static public void MoveCam(BATTLECAMTYPE eType, float duration, Ease easeType = Ease.OutCubic)
    {
        //bm.StartCoroutine(bm.m_followMoving.MoveCam(eType, fWait, fMove));
        //bm.StartCoroutine(bm.m_lookAtMoving.MoveCam(eType, fWait, fMove));

        var follow = bm.m_trFollow[(int)eType];
        var lookAt = bm.m_trLookAt[(int)eType];

        bm.cinemachineMoving.ChangeCameraTargetSmooth(bm.m_trInitFollow, follow, bm.m_trInitLookAt, lookAt, duration, easeType);
    }

    static public void SelectEnemy(int idx, GameObject go)
    {
        bool isCheck = false;

        //��Ʋ ��� ���Ե� ������ Ȯ��
        foreach(var info in m_listBattleInfo)
        {
            if(info.go == go)
            {
                isCheck = true;
                break;
            }
        }

        if (!isCheck) return;

        if (m_bSelectedAll)
        {
            for(int i = 0; i < GameManager.gm.m_listEnemyInfo.Count; i++)
            {
                bm.m_arrSelectedObjs[i].gameObject.SetActive(true);
            }

            for (int i = 0; i < m_listBattleTimelineInfo.Count; i++)
            {
                var info = m_listBattleTimelineInfo[i];

                if (i >= GameManager.gm.m_listOrderInfo.Count) continue;

                if (info.go == go)
                {
                    info.go.GetComponent<EnemyFSM>().m_bSelected = true;
                }


                if (!info.bUser)
                {
                    GameManager.gm.m_listOrderInfo[i].GetComponent<OrderInfo>().SetTarget(true);
                }
            }

            return;
        }

        foreach (var select in bm.m_arrSelectedObjs)
        {
            select.gameObject.SetActive(false);
        }

        bm.m_arrSelectedObjs[idx].SetActive(true);

        for(int i = 0; i < m_listBattleTimelineInfo.Count; i++)
        {

            var info = m_listBattleTimelineInfo[i];
            bool isTarget = false;
            
            if(!info.bUser)
                info.go.GetComponent<EnemyFSM>().m_bSelected = false;

            if (info.go == go)
            {
                info.go.GetComponent<EnemyFSM>().m_bSelected = true;
                isTarget = true;
            }

            if (i >= GameManager.gm.m_listOrderInfo.Count) continue;
            GameManager.gm.m_listOrderInfo[i].GetComponent<OrderInfo>().SetTarget(isTarget);
        }
    }

    static public void FaintingTarget(int targetIdx, bool isUser)
    {
        //�켱 ������ �ƴϳĿ� ���� �޶���
        //Ÿ�� ���ʹ� �ε����� ��������
        //���ӸŴ������� ���ʹ̸���Ʈ�� �ҷ��� �񱳰����ϰ�
        //���ʹ� ���� �����ͼ�
        //��ƲŸ�Ӷ��ο��� ���ϰ� �´��ϸ�װ� ����
        if(isUser)
        {
            var player = GameManager.gm.m_listPlayerInfo[targetIdx];

            int deleteIdx = 0;

            foreach (var info in m_listBattleTimelineInfo)
            {
                if (info.go.GetComponent<InGamePlayerController>() == player)
                {
                    deleteIdx = m_listBattleTimelineInfo.IndexOf(info);
                    break;
                }
            }

            m_listBattleTimelineInfo.RemoveAt(deleteIdx);
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(GameManager.gm.m_listOrderInfo[deleteIdx].gameObject);
        }

        else
        {
            var enemy = GameManager.gm.m_listEnemyInfo[targetIdx];

            int deleteIdx = 0;

            foreach(var info in m_listBattleTimelineInfo)
            {
                if (info.go.GetComponent<EnemyFSM>() == enemy)
                {
                    deleteIdx = m_listBattleTimelineInfo.IndexOf(info);
                    break;
                }
            }

            m_listBattleTimelineInfo.RemoveAt(deleteIdx);
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(GameManager.gm.m_listOrderInfo[deleteIdx].gameObject);
        }
    }

    static public void InitBattleTimeline()
    {
        m_listReadyBattleTime.Clear();

        //��Ʋ�� ���� �ֵ��� ���� �ӵ��� ���� ��Ʈ�ϰ�
        for(int i = 0; i < m_listBattleInfo.Count; i++)
        {
           var info = m_listBattleInfo[i];

            info.iRegistrationOrder = i;
            m_listBattleInfo[i] = info;
        }

        //���ǵ� ���� -> ��� ���� ����
        m_listBattleInfo = m_listBattleInfo.OrderByDescending(c => c.SPD).
            ThenBy(c => c.iRegistrationOrder).ToList();

        //�ӵ��� 1.5f�̻��̸� �������� ������ �� �ִ�. Ÿ�Ӷ��� �����ߵ�
        var players = m_listBattleInfo.FindAll(b => b.bUser == true);
        var enemies = m_listBattleInfo.FindAll(b => b.bUser == false);

        //���ʹ� ��հ��� ���Ѵ�?...
        float averageEnemySpeed = enemies.Average(b => b.SPD);
        float averagePlayerSpeed = players.Average(b => b.SPD);

        int idx = 0;
        while(m_listBattleTimelineInfo.Count < 12)
        {
            m_listBattleTimelineInfo.Add(m_listBattleInfo[idx]);

            bool isUser = m_listBattleInfo[idx].bUser ? true : false;
            float oppenentAvg = isUser ? averageEnemySpeed : averagePlayerSpeed;

            if (m_listBattleTimelineInfo.Count >= 12) break;

            if (m_listBattleInfo[idx].SPD >= oppenentAvg * 1.5f)
            {
                m_listBattleTimelineInfo.Add(m_listBattleInfo[idx]);
            }

            idx++;
            if (idx >= m_listBattleInfo.Count) idx = 0;
        }

        //���������� ������ ��� �س���
        //m_goNextOrder = m_listBattleInfo[idx].go;
        //�Ȱ������� ��ġ�سֱ�
        m_listReadyBattleTime.AddRange(m_listBattleTimelineInfo);

    }

    static public void RefreshTimeline()
    {
        //���� �ٽ��ѹ� ���ߵ� ��Ȯ�� ����� �´���
        //�ֳĸ� ���� ����Ʈ�� ���� �������� ���� �������� �����ϰŵ�
        //�װ� Ȯ���ϰ� ����ϰ� ���ߵ�
        //�ϴ� ��հ����� ��� ����.
        
        //���� �غ�� ��ƲŸ�Ӷ����� ī��Ʈ�� �´��� Ȯ���Ѵ�.
        //�ٽ� ��Ʈ�ؼ� �����ߵǳ�. ������.
        //���⼭ 12������ �Ʒ���°� �׾��ų� �ټ��� ���� �ɷȴٴ� ��
        if(m_listReadyBattleTime.Count < 12)
        {
            m_listReadyBattleTime.Clear();

            m_listBattleInfo = m_listBattleInfo.OrderByDescending(c => c.SPD).
                ThenBy(c => c.iRegistrationOrder).ToList();

            //�ӵ��� 1.5f�̻��̸� �������� ������ �� �ִ�. Ÿ�Ӷ��� �����ߵ�
            var players = m_listBattleInfo.FindAll(b => b.bUser == true);
            var enemies = m_listBattleInfo.FindAll(b => b.bUser == false);

            //���ʹ� ��հ��� ���Ѵ�?...
            float averageEnemySpeed = enemies.Average(b => b.SPD);
            float averagePlayerSpeed = players.Average(b => b.SPD);

            int newidx = 0;
            while (m_listReadyBattleTime.Count < 12)
            {
                m_listReadyBattleTime.Add(m_listBattleInfo[newidx]);

                bool isUser = m_listBattleInfo[newidx].bUser ? true : false;
                float oppenentAvg = isUser ? averageEnemySpeed : averagePlayerSpeed;

                if (m_listReadyBattleTime.Count >= 12) break;

                if (m_listBattleInfo[newidx].SPD >= oppenentAvg * 1.5f)
                {
                    m_listReadyBattleTime.Add(m_listBattleInfo[newidx]);
                }

                newidx++;
                if (newidx >= m_listBattleInfo.Count) newidx = 0;
            }
        }

        //���� idx���� �˾ƾ��Ѵ�.
        int orderNum = m_listBattleTimelineInfo.Count;
        int idx = 0;

        while (m_listBattleTimelineInfo.Count < 12)
        {
            //UI�� ��� Ÿ�Ӷ��� �÷�������
            m_listBattleTimelineInfo.Add(m_listReadyBattleTime[idx]);

            //�غ�Ǿ� �ִ� Ÿ�Ӷ��� �������� �ֱ�
            m_listReadyBattleTime.Add(m_listReadyBattleTime[idx]);

            //�ش� ��ƲŸ�� ������ �����Ѵ�.
            m_listReadyBattleTime.RemoveAt(idx);

            idx++;
        }


        if (PhotonNetwork.IsMasterClient)
            GameManager.m_curUI.GetComponent<UIManagerInGame>().m_orderNum = orderNum;

        //Debug.Log("�ѹ� : " + GameManager.m_curUI.GetComponent<UIManagerInGame>().m_orderNum);
        //Debug.Log("�ִ밳�� : " + BattleManager.m_listBattleTimelineInfo.Count);

        if (PhotonNetwork.IsMasterClient)
            bm.StartCoroutine(GameManager.m_curUI.GetComponent<UIManagerInGame>().InstantiateOrderImg());
    }

    static public void ShakeCamera(float fIntensity, float fTime)
    {
        bm.m_goVirCam.ShakeCamera(fIntensity, fTime);
    }

    static public void CheckNextBattle()
    {
        //�������� ������ ����� üũ
        if (m_listBattleTimelineInfo[0].bUser)
        {
            m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>().TickTurnBuffCheck();
        }

        else
        {
            m_listBattleTimelineInfo[0].go.GetComponent<EnemyFSM>().TickTurnBuffCheck();
        }

        bool isEnemyClear = true;
        bool isUserClear = false;

        foreach (var fighter in m_listBattleInfo)
        {
            if (!fighter.bUser) isEnemyClear = false;
        }

        var player = GameManager.gm.m_listPlayerInfo.Find(b => b.m_ragdollObj.activeSelf);

        if (player != null) isUserClear = true;

        //���ʹ̿� �÷��̾ ��� ����ִ�. �� ���� é��
        if (!isEnemyClear && !isUserClear)
        {
            NetworkManager.nm.PV.RPC("DestroyOrderInfoRPC", RpcTarget.MasterClient);
            NetworkManager.nm.PV.RPC("RefreshBattleTimelineRPC", RpcTarget.All);
        }

        //������ �Ѹ��̶� ������ ��
        else if (isUserClear)
        {
            //Debug.Log("���� ��� �׾��� ����");
            //���ӿ����ߴٰ� ����ߵ�
            //������ �ٽ� ���ư��ߵ�
            NetworkManager.nm.PV.RPC("DestroyOrderInfoRPC", RpcTarget.MasterClient);
            NetworkManager.nm.PV.RPC("ReturnRoomRPC", RpcTarget.All);
        }

        else if (isEnemyClear)
        {
            //�̰� ���ξ��̰� �������� �˷���ߵ� ����?
            //���⼭ ó���ؾߵǳ�
            NetworkManager.nm.PV.RPC("RewardTimePRC", RpcTarget.All);
        }
    }

    static public void DefeatToReturnRoom()
    {
        bm.StartCoroutine(GameManager.m_curUI.GetComponent<UIManagerInGame>().ShowDefeat());

        DeleteBattleInfo();
    }

    static void DeleteBattleInfo()
    {
        //��� �ʱ�ȭ
        m_iCurFloor = 0;
        InvenManager.ClearInven();

        if (GameManager.gm.m_inGamePlayer.m_pv.IsMine)
        {
            //���� ������ Ŭ�����Ѵ�
            GameManager.gm.m_inGamePlayer.m_pv.RPC("ClearUserInfoRPC", RpcTarget.All,
                GameManager.gm.m_BattlePosIdx);
        }

        //���ʹ̿� Ȱ��ȭ�� ������Ʈ�� ��Ȱ��ȭ���Ѿߵ�
        foreach (var enemy in GameManager.gm.m_listEnemyInfo)
        {
            enemy.InActiveObj();
        }

        m_listBattleTimelineInfo.Clear();
        m_listBattleInfo.Clear();
    }

    static public void RewardCheckTime()
    {
        bm.m_stateMachine.ChangeState(BATTLESTATE.REWARDCHECK);
    }
}
