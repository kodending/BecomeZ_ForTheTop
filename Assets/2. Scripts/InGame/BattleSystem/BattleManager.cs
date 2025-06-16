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
        ////테스트 용도
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
        //배틀에 들어온 애들을 먼저 속도에 따라 소트하고
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
        //여기 다시한번 봐야됨 정확한 계산이 맞는지
        //왜냐면 내가 리스트에 넣은 마지막놈 다음 순서부터 들어가야하거든
        //그걸 확실하게 계산하고 들어가야됨
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

        //Debug.Log("넘버 : " + GameManager.m_curUI.GetComponent<UIManagerInGame>().m_orderNum);
        //Debug.Log("최대개수 : " + BattleManager.m_listBattleTimelineInfo.Count);

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

        //에너미와 플레이어가 모두 살아있다. 면 다음 챕터
        if (!isEnemyClear && !isUserClear)
        {
            //순서에서 0번째 빼야하고
            //다음 순서 넣어야하고
            //UI도 바꿔야됨ㅑㅣ.
            PhotonNetwork.Destroy(GameManager.gm.m_listOrderInfo[0].gameObject);
            //BattleManager.m_listBattleTimelineInfo.RemoveAt(0);
            //BattleManager.RefreshTimeline();
            //BattleManager.bm.m_stateMachine.ChangeState(BATTLESTATE.SELECT);

            NetworkManager.nm.PV.RPC("RefreshBattleTimelineRPC", RpcTarget.All);
        }

        else if (isUserClear)
        {
            Debug.Log("유저 모두 죽었어 ㅎㅎ");
        }

        else if (isEnemyClear)
        {
            //에너미가 전원 죽었다. 그럼 보상을 받고 점검을 해야겠지??
            //승리 UI 띄우고 // 보상체크로 넘어가기
            bm.m_stateMachine.ChangeState(BATTLESTATE.REWARDCHECK);
        }
    }
}
