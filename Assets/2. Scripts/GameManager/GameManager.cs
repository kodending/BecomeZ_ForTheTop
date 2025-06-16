using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager gm;

    [HideInInspector]
    public GMStateMachine m_stateMachine { get; private set; }

    public static GAMESTATE m_curState;

    public static GameObject m_curUI;

    public LobbyRoomInfo m_curRoomInfo;

    public string m_curRoomName;
    public string m_curRoomPassword;

    public bool m_bRoomCreator;

    public LobbyPlayerController m_lobbyPlayer;
    public InGamePlayerController m_inGamePlayer;
    public int m_BattlePosIdx; // 배틀 위치 인덱스
    public int m_SavedClassIdx; // 저장된 클래스 인덱스
    public List<OrderInfo> m_listOrderInfo = new List<OrderInfo>();
    public List<EnemyFSM> m_listEnemyInfo = new List<EnemyFSM>();
    public List<InGamePlayerController> m_listPlayerInfo = new List<InGamePlayerController>();

    private void Awake()
    {
        gm = this;
        DontDestroyOnLoad(gm);

        //debug용
        Screen.SetResolution(960, 540, false);
        InitStateMachine();
    }

    private void Update()
    {
        m_stateMachine?.UpdateState();
    }

    private void FixedUpdate()
    {
        m_stateMachine.FixedUpdateState();
    }

    void InitStateMachine()
    {
        m_stateMachine = new GMStateMachine(GAMESTATE.LOAD, gameObject.AddComponent<GameInLoad>());
        m_stateMachine.AddState(GAMESTATE.MAIN, gameObject.AddComponent<GameInMain>());
        m_stateMachine.AddState(GAMESTATE.LOBBY, gameObject.AddComponent<GameInLobby>());
        m_stateMachine.AddState(GAMESTATE.WAITROOM, gameObject.AddComponent<GameInWait>());
        m_stateMachine.AddState(GAMESTATE.INGAME, gameObject.AddComponent<GameInGame>());

        m_stateMachine.currentState?.OnEnterState();
    }

    static public IEnumerator ChangeScene(string sceneName, GAMESTATE eState, float fWaitTime)
    {
        yield return new WaitForSeconds(fWaitTime);

        SceneManager.LoadScene(sceneName);
        UIManager.StopSequence();

        yield return new WaitForSeconds(0.5f);
        gm.m_stateMachine.ChangeState(eState);
    }

    static public void LobbyPlayerChat(string strChat)
    {
        NetworkManager.SendChat(strChat, gm.m_lobbyPlayer.m_pv.ViewID);
    }
}
