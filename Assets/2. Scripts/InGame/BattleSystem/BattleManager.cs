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

    //현재 몇층에 있는지 확인하기 위함
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

        //배틀 대상에 포함된 놈인지 확인
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
        //우선 유저냐 아니냐에 따라 달라짐
        //타겟 에너미 인덱스를 가져오면
        //게임매니저에서 에너미리스트를 불러와 비교가능하고
        //에너미 정보 가져와서
        //배틀타임라인에서 비교하고 맞다하면그거 삭제
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

        //배틀에 들어온 애들을 먼저 속도에 따라 소트하고
        for(int i = 0; i < m_listBattleInfo.Count; i++)
        {
           var info = m_listBattleInfo[i];

            info.iRegistrationOrder = i;
            m_listBattleInfo[i] = info;
        }

        //스피드 순서 -> 등록 순서 기준
        m_listBattleInfo = m_listBattleInfo.OrderByDescending(c => c.SPD).
            ThenBy(c => c.iRegistrationOrder).ToList();

        //속도에 1.5f이상이면 연속으로 공격할 수 있다. 타임라인 만들어야됨
        var players = m_listBattleInfo.FindAll(b => b.bUser == true);
        var enemies = m_listBattleInfo.FindAll(b => b.bUser == false);

        //에너미 평균값을 구한다?...
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

        //다음순서가 누군지 기록 해놓기
        //m_goNextOrder = m_listBattleInfo[idx].go;
        //똑같은순서 배치해넣기
        m_listReadyBattleTime.AddRange(m_listBattleTimelineInfo);

    }

    static public void RefreshTimeline()
    {
        //여기 다시한번 봐야됨 정확한 계산이 맞는지
        //왜냐면 내가 리스트에 넣은 마지막놈 다음 순서부터 들어가야하거든
        //그걸 확실하게 계산하고 들어가야됨
        //일단 평균값부터 계산 들어가자.
        
        //먼저 준비된 배틀타임라인이 카운트가 맞는지 확인한다.
        //다시 소트해서 만들어야되네. 죽으면.
        //여기서 12개보다 아래라는건 죽었거나 다수가 기절 걸렸다는 뜻
        if(m_listReadyBattleTime.Count < 12)
        {
            m_listReadyBattleTime.Clear();

            m_listBattleInfo = m_listBattleInfo.OrderByDescending(c => c.SPD).
                ThenBy(c => c.iRegistrationOrder).ToList();

            //속도에 1.5f이상이면 연속으로 공격할 수 있다. 타임라인 만들어야됨
            var players = m_listBattleInfo.FindAll(b => b.bUser == true);
            var enemies = m_listBattleInfo.FindAll(b => b.bUser == false);

            //에너미 평균값을 구한다?...
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

        //현재 idx값을 알아야한다.
        int orderNum = m_listBattleTimelineInfo.Count;
        int idx = 0;

        while (m_listBattleTimelineInfo.Count < 12)
        {
            //UI에 띄울 타임라인 올려버리기
            m_listBattleTimelineInfo.Add(m_listReadyBattleTime[idx]);

            //준비되어 있는 타임라인 마지막에 넣기
            m_listReadyBattleTime.Add(m_listReadyBattleTime[idx]);

            //해당 배틀타임 순번은 삭제한다.
            m_listReadyBattleTime.RemoveAt(idx);

            idx++;
        }


        if (PhotonNetwork.IsMasterClient)
            GameManager.m_curUI.GetComponent<UIManagerInGame>().m_orderNum = orderNum;

        //Debug.Log("넘버 : " + GameManager.m_curUI.GetComponent<UIManagerInGame>().m_orderNum);
        //Debug.Log("최대개수 : " + BattleManager.m_listBattleTimelineInfo.Count);

        if (PhotonNetwork.IsMasterClient)
            bm.StartCoroutine(GameManager.m_curUI.GetComponent<UIManagerInGame>().InstantiateOrderImg());
    }

    static public void ShakeCamera(float fIntensity, float fTime)
    {
        bm.m_goVirCam.ShakeCamera(fIntensity, fTime);
    }

    static public void CheckNextBattle()
    {
        //본인차례 끝나고 디버프 체크
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

        //에너미와 플레이어가 모두 살아있다. 면 다음 챕터
        if (!isEnemyClear && !isUserClear)
        {
            NetworkManager.nm.PV.RPC("DestroyOrderInfoRPC", RpcTarget.MasterClient);
            NetworkManager.nm.PV.RPC("RefreshBattleTimelineRPC", RpcTarget.All);
        }

        //유저가 한명이라도 죽으면 끝
        else if (isUserClear)
        {
            //Debug.Log("유저 모두 죽었어 ㅎㅎ");
            //게임오버했다고 알줘야됨
            //룸으로 다시 돌아가야됨
            NetworkManager.nm.PV.RPC("DestroyOrderInfoRPC", RpcTarget.MasterClient);
            NetworkManager.nm.PV.RPC("ReturnRoomRPC", RpcTarget.All);
        }

        else if (isEnemyClear)
        {
            //이걸 죽인아이가 전원한테 알려줘야됨 ㅇㅋ?
            //여기서 처리해야되네
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
        //모두 초기화
        m_iCurFloor = 0;
        InvenManager.ClearInven();

        if (GameManager.gm.m_inGamePlayer.m_pv.IsMine)
        {
            //먼저 정보를 클리어한다
            GameManager.gm.m_inGamePlayer.m_pv.RPC("ClearUserInfoRPC", RpcTarget.All,
                GameManager.gm.m_BattlePosIdx);
        }

        //에너미에 활성화된 오브젝트를 비활성화시켜야됨
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
