using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Specialized;
using Unity.VisualScripting;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager nm;
    public PhotonView PV;

    [Header("Playfab 연동관련")]
    public PlayerLeaderboardEntry m_myPlayFabInfo = new PlayerLeaderboardEntry();
    public List<PlayerLeaderboardEntry> m_listPlayFabUser = new List<PlayerLeaderboardEntry>();
    public string m_strMail, m_strPW, m_strName;

    [Header("Photon 관련")]
    public List<RoomInfo> m_listRoomInfo = new List<RoomInfo>();

    private void Awake()
    {
        nm = this;
        DontDestroyOnLoad(nm);

        //동기화를 좀더 빠르게 하기 위함
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    #region 로그인 함수
    static public bool CheckLogin()
    {
        if (nm.m_myPlayFabInfo.PlayFabId == "") return false;
        else return true;
    }

    static public void Login(string i_strMail, string i_strPw)
    {
        var req = new LoginWithEmailAddressRequest { Email = i_strMail, Password = i_strPw };
        PlayFabClientAPI.LoginWithEmailAddress(req,
            (result) => { Debug.Log("로그인 성공"); GetLeaderboard(result.PlayFabId); nm.StartCoroutine(GameManager.m_curUI.GetComponent<UIManagerInMain>().OnSuccessLogin()); },
            (error) => { Debug.Log("로그인 실패"); });
    }

    static void GetLeaderboard(string myID)
    {
        nm.m_listPlayFabUser.Clear();

        for (int i = 0; i < 100; i++)
        {
            var request = new GetLeaderboardRequest
            {
                StartPosition = i * 100,
                StatisticName = "IDInfo",
                MaxResultsCount = 100,
                ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
            };

            PlayFabClientAPI.GetLeaderboard(request, (result) =>
            {
                if (result.Leaderboard.Count == 0) return;
                for (int j = 0; j < result.Leaderboard.Count; j++)
                {
                    nm.m_listPlayFabUser.Add(result.Leaderboard[j]);
                    if (result.Leaderboard[j].PlayFabId == myID) nm.m_myPlayFabInfo = result.Leaderboard[j];
                }
            },
            (error) => { Debug.Log("리더보드 실패"); });
        }
    }

    static public void Connet() => PhotonNetwork.ConnectUsingSettings();

    //로비에 바로 접속 되도록 함수 구현
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        StartCoroutine(GameManager.ChangeScene("LobbyScene", GAMESTATE.LOBBY, 1f));
        Debug.Log("로비 접속 완료");
    }

    #endregion

    #region 회원가입 함수
    static public void Register(string i_strMail, string i_strPW, string i_strID)
    {
        var request = new RegisterPlayFabUserRequest { Email = i_strMail, Password = i_strPW, Username = i_strID, DisplayName = i_strID };
        PlayFabClientAPI.RegisterPlayFabUser(request,
            (result) => { print("회원가입 성공!"); GameManager.m_curUI.GetComponent<UIManagerInMain>().OnSuccessSignUp(); nm.SetStat(); },
            (error) => { print("회원가입 실패"); nm.RegisterErrorMessage(error.GenerateErrorReport()); });
    }

    void RegisterErrorMessage(string msg)
    {
        string strMsg = "";

        if(msg.Contains("Email") && msg.Contains("exists"))
        {
            strMsg = "메일이 이미 존재합니다";
        }

        else if (msg.Contains("Email") && msg.Contains("valid"))
        {
            strMsg = "잘못된 메일정보입니다";
        }

        else if (msg.Contains("Username") && msg.Contains("exists"))
        {
            strMsg = "아이디가 이미 존재합니다";
        }

        else if (msg.Contains("Username") && msg.Contains("between"))
        {
            strMsg = "아이디는 3자 이상 입력해야합니다";
        }

        else if (msg.Contains("Password") && msg.Contains("between"))
        {
            strMsg = "비밀번호는 6자 이상 입력해야합니다";
        }

        else
        {
            Debug.Log(msg);
        }

        UIManager.SystemMsg(strMsg);
    }

    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => print("값 저장실패"));
    }
    #endregion

    #region 로그아웃 함수
    static public void DisConnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        StartCoroutine(GameManager.ChangeScene("MainScene", GAMESTATE.MAIN, 1.5f));
    }
    #endregion

    #region 방 생성 및 입장 및 방떠나기 함수
    static public void CreateRoom(string name, string password)
    {
        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        string roomName = "";

        if (name == "")
        {
            roomName = nm.m_myPlayFabInfo.DisplayName + "님의 방";
        }

        else roomName = name;

        if (password == "")
        {
            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        }

        else
        {
            Hashtable hash = new Hashtable();
            hash["password"] = password;

            roomOptions.CustomRoomProperties = hash;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };

            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        }
    }

    static public void JoinRoom(string roomName)
    {
        Debug.Log(roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    //입장 성공했을 때 호출되는 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공");
        GameObject lobbyPlayer = PhotonNetwork.Instantiate("LOBBYPLAYER_1", new Vector3(0, 0.012f, 0), Quaternion.Euler(0, 180, 0));
        lobbyPlayer.GetComponent<LobbyPlayerController>().InitInfo();
        lobbyPlayer.SetActive(true);
    }

    static public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion

    #region 방 리스트 업데이트
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;

        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!m_listRoomInfo.Contains(roomList[i])) m_listRoomInfo.Add(roomList[i]);
                else m_listRoomInfo[m_listRoomInfo.IndexOf(roomList[i])] = roomList[i];
            }

            else if (m_listRoomInfo.IndexOf(roomList[i]) != -1)
                m_listRoomInfo.RemoveAt(m_listRoomInfo.IndexOf(roomList[i]));
        }
    }
    #endregion

    #region 채팅 기능
    static public void SendChat(string strChat, int viewID)
    {
        nm.PV.RPC("SendChatRPC", RpcTarget.All, strChat, viewID);
    }

    [PunRPC]
    void SendChatRPC(string strChat, int viewID)
    {
        LobbyPlayerController[] players = PunPoolManager.ppm.m_goLobbyParent.GetComponentsInChildren<LobbyPlayerController>();

        foreach (var player in players)
        {
            if(player.m_pv.ViewID == viewID)
            {
                player.m_playerCanvas.ChatText(strChat);
                UIManager.ShowChat(player.m_playerCanvas.m_imgChat.gameObject);
                break;
            }
        }
    }
    #endregion

    #region 게임 시작
    [PunRPC]
    public void GameStart()
    {
        StartCoroutine(GameManager.m_curUI.GetComponent<UIManagerInWait>().GameStart());
    }
    #endregion

    #region 배틀타임라인 동기화
    [PunRPC]
    public void InitBattleTimelineRPC()
    {
        BattleManager.InitBattleTimeline();
    }

    [PunRPC]
    public void RefreshBattleTimelineRPC()
    {
        BattleManager.m_listBattleTimelineInfo.RemoveAt(0);
        BattleManager.RefreshTimeline();
        BattleManager.bm.m_stateMachine.ChangeState(BATTLESTATE.SELECT);
    }
    #endregion

    #region 마스터한테 하는 요청사항들
    [PunRPC]
    public void DestroyOrderInfoRPC()
    {
        PhotonNetwork.Destroy(GameManager.gm.m_listOrderInfo[0].gameObject);
    }
    #endregion

    #region 공격 성공여부 전체 HUD 보이게하기
    [PunRPC]
    public void UserAttackCalHudRPC(int idx, bool bSuccess)
    {
        GameManager.m_curUI.GetComponent<UIManagerInGame>().ApplyUserAttackUI(idx, bSuccess);
    }

    [PunRPC]
    public void InitUserAttackCalHudRPC()
    {
        GameManager.m_curUI.GetComponent<UIManagerInGame>().InitUserAttackUI();
    }

    [PunRPC]
    public void UserAttackDamageUI(int damage, int enemyIdx, int atkResult)
    {
        GameManager.m_curUI.GetComponent<UIManagerInGame>().m_arrEnemyCanvas[enemyIdx].OnDamage(damage, atkResult);

        //VFX도 여기서 띄우자.
        //이펙트가
        //에너미 한테 직접 터지는게 있을거고
        //이동하는 오브젝트가 있을것이고
        //플레이어 주변에 생기는 이펙트도 있을것이다.
        //이걸 구분할 수 있어야 한다.
    }

    [PunRPC]
    public void EnemyAttackCalHudRPC(int idx, bool bSuccess)
    {
        GameManager.m_curUI.GetComponent<UIManagerInGame>().ApplyEnemyAttackUI(idx, bSuccess);
    }

    [PunRPC]
    public void InitEnemyAttackCalHudRPC()
    {
        GameManager.m_curUI.GetComponent<UIManagerInGame>().InitEnemyAttackUI();
    }

    [PunRPC]
    public void EnemyAttackDamageUI(int damage, int playerIdx, int atkResult)
    {
        GameManager.m_curUI.GetComponent<UIManagerInGame>().m_arrUserCanvas[playerIdx].OnDamage(damage, atkResult);
    }

    #endregion
}
