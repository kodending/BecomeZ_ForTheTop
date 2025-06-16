using Photon.Pun;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
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
    GameObject m_goNextOrder;

    public List<Color> m_listUserColors;
    public List<Color> m_listAttributeColors;
    public List<Sprite> m_listAttributeSprites;

    [SerializeField] CinemachineShake m_goVirCam;

    static public int m_iCurSelectedSkill = 0;

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

    static public void MoveCam(BATTLECAMTYPE eType, float fWait, float fMove)
    {
        bm.StartCoroutine(bm.m_followMoving.MoveCam(eType, fWait, fMove));
        bm.StartCoroutine(bm.m_lookAtMoving.MoveCam(eType, fWait, fMove));
    }

    static public void InitBattleTimeline()
    {
        //��Ʋ�� ���� �ֵ��� ���� �ӵ��� ���� ��Ʈ�ϰ�
        m_listBattleInfo.Sort((BATTLEINFO a, BATTLEINFO b) => { return b.SPD.CompareTo(a.SPD); });

        int idx = 0;
        for (int i = 0; i < 12; i++)
        {
            idx = i % m_listBattleInfo.Count;
            m_listBattleTimelineInfo.Add(m_listBattleInfo[idx]);
        }

        idx++;
        if (idx >= m_listBattleInfo.Count) idx = 0;

        bm.m_goNextOrder = m_listBattleInfo[idx].go;
    }

    static public void KillEnemy(GameObject enemy)
    {
        foreach(var info in m_listBattleInfo)
        {
            if(info.go == enemy)
            {
                m_listBattleInfo.Remove(info);
                break;
            }
        }

        for (int i = 0; i < m_listBattleTimelineInfo.Count; i++)
        {
            var info = m_listBattleTimelineInfo[i];

            if (info.go == enemy)
            {
                m_listBattleTimelineInfo.Remove(info);
                PhotonNetwork.Destroy(GameManager.gm.m_listOrderInfo[i].gameObject);
            }
        }

        RefreshTimeline();

        PhotonNetwork.Destroy(enemy);
    }

    static public void SelectEnemy(int idx, GameObject go)
    {
        bool isCheck = false;

        foreach(var info in m_listBattleInfo)
        {
            if(info.go == go)
            {
                isCheck = true;
                break;
            }
        }

        if (!isCheck) return;

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

    static public void RefreshTimeline()
    {
        //���� �ٽ��ѹ� ���ߵ� ��Ȯ�� ����� �´���
        //�ֳĸ� ���� ����Ʈ�� ���� �������� ���� �������� �����ϰŵ�
        //�װ� Ȯ���ϰ� ����ϰ� ���ߵ�
        int curIdx = 0;
        for (; curIdx < m_listBattleInfo.Count; curIdx++)
        {
            if (m_listBattleInfo[curIdx].go == bm.m_goNextOrder) break;
        }

        int timelineCount = m_listBattleTimelineInfo.Count;


        for (int i = timelineCount; i < 12; i++)
        {
            if (curIdx >= m_listBattleInfo.Count) curIdx = 0;
            m_listBattleTimelineInfo.Add(m_listBattleInfo[curIdx]);
            curIdx++;
        }

        if (curIdx >= m_listBattleInfo.Count) curIdx = 0;

        bm.m_goNextOrder = m_listBattleInfo[curIdx].go;

        if (PhotonNetwork.IsMasterClient)
            GameManager.m_curUI.GetComponent<UIManagerInGame>().m_orderNum = timelineCount;

        //Debug.Log("�ѹ� : " + GameManager.m_curUI.GetComponent<UIManagerInGame>().m_orderNum);
        //Debug.Log("�ִ밳�� : " + BattleManager.m_listBattleTimelineInfo.Count);

        if(PhotonNetwork.IsMasterClient)
            bm.StartCoroutine(GameManager.m_curUI.GetComponent<UIManagerInGame>().InstantiateOrderImg());
    }

    static public void ShakeCamera(float fIntensity, float fTime)
    {
        bm.m_goVirCam.ShakeCamera(fIntensity, fTime);
    }

    static public void CheckNextBattle()
    {
        bool isEnemyClear = true;
        bool isUserClear = true;

        foreach (var fighter in m_listBattleInfo)
        {
            if (!fighter.bUser) isEnemyClear = false;
            else if (fighter.bUser) isUserClear = false;
        }

        //���ʹ̿� �÷��̾ ��� ����ִ�. �� ���� é��
        if (!isEnemyClear && !isUserClear)
        {
            //�������� 0��° �����ϰ�
            //���� ���� �־���ϰ�
            //UI�� �ٲ�ߵʤ���.
            PhotonNetwork.Destroy(GameManager.gm.m_listOrderInfo[0].gameObject);
            //BattleManager.m_listBattleTimelineInfo.RemoveAt(0);
            //BattleManager.RefreshTimeline();
            //BattleManager.bm.m_stateMachine.ChangeState(BATTLESTATE.SELECT);

            NetworkManager.nm.PV.RPC("RefreshBattleTimelineRPC", RpcTarget.All);
        }

        else if (isUserClear)
        {
            Debug.Log("���� ��� �׾��� ����");
        }

        else if (isEnemyClear)
        {
            //���ʹ̰� ���� �׾���. �׷� ������ �ް� ������ �ؾ߰���??
            //�¸� UI ���� // ����üũ�� �Ѿ��
            bm.m_stateMachine.ChangeState(BATTLESTATE.REWARDCHECK);
        }
    }
}
