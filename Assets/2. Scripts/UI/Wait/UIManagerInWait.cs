using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerInWait : MonoBehaviourPunCallbacks
{
    [SerializeField] Text m_txtWait;
    [SerializeField] Text m_txtSubTitle;
    [SerializeField] Text m_txtReturnLobby;
    [SerializeField] Image m_imgDarkChange;
    [SerializeField] Image m_imgReturnLobbyPanel;
    [SerializeField] GameObject m_goJoy;

    public CinemachineMoving cinemachineMoving;
    public Transform m_newFollow;
    public Transform m_InitFollow;
    public Transform m_newLookAt;
    public Transform m_InitLookAt;

    #region 채팅관련
    [SerializeField] GameObject m_goChatPanel;
    [SerializeField] InputField m_inputChat;
    #endregion

    #region 클래스 변경 관련
    [SerializeField] Image m_imgClassButton;
    public ClassAvatar m_classAvatar;
    [SerializeField] RectTransform[] m_trMaxStatPoints;
    [SerializeField] RectTransform[] m_trCurStatPoints;
    [SerializeField] LineRenderer m_lineStats;
    [SerializeField] GameObject m_goClassPanel;
    public Text[] m_arrStatTexts;
    public Text m_txtClassName;
    #endregion

    #region 옵션 관련
    [SerializeField] Image m_imgSettingButton;
    #endregion

    //#region 카메라 이동관련
    //[SerializeField] CamMoving m_camMoving;
    //[SerializeField] CamMoving m_camLookMoving;
    //#endregion

    #region 게임시작 관련
    [SerializeField] Text m_txtGameStart;
    bool m_isStarted = false;
    #endregion

    void Start()
    {
        GameManager.m_curUI = GameObject.Find("UIManagerInWait");

        StartCoroutine(ActiveUI());
    }

    private void Update()
    {
        if(m_goClassPanel.activeSelf)
        {
            for (int i = 0; i < m_trCurStatPoints.Length; i++)
            {
                var tr = m_trCurStatPoints[i];
                m_lineStats.SetPosition(i, new Vector3(tr.localPosition.x, tr.localPosition.y, 0));
            }

            int idx = m_trCurStatPoints.Length;
            m_lineStats.SetPosition(idx, new Vector3(m_trCurStatPoints[0].localPosition.x, m_trCurStatPoints[0].localPosition.y, 0));
        }
    }

    IEnumerator ActiveUI()
    {
        //StartCoroutine(m_camLookMoving.MoveCam());


        UIManager.FadeOutImage(m_imgDarkChange, 1f);

        yield return new WaitForSeconds(0.5f);

        UIManager.FadeInOutText(m_txtWait, 3f);

        yield return new WaitForSeconds(1.0f);

        UIManager.FadeInOutText(m_txtSubTitle, 2f);

        yield return new WaitForSeconds(1.0f);

        cinemachineMoving.ChangeCameraTargetSmooth(m_InitFollow, m_InitFollow, m_InitLookAt, m_newLookAt, 2f, Ease.InQuart);

        yield return new WaitForSeconds(3f);

        UIManager.ShowScale(m_txtReturnLobby.gameObject);
        UIManager.ShowScale(m_goJoy);
        UIManager.ShowScale(m_goChatPanel);
        UIManager.ShowScale(m_imgClassButton.gameObject);
        UIManager.ShowScale(m_imgSettingButton.gameObject);

        if(PhotonNetwork.IsMasterClient)
            UIManager.ShowScale(m_txtGameStart.gameObject);
    }

    public void OnClickReturnLobby()
    {
        if (m_imgReturnLobbyPanel.gameObject.activeSelf) return;

        UIManager.ShowScale(m_imgReturnLobbyPanel.gameObject);
    }

    //로비로 돌아가겠냐는 메세지에 확인버튼
    public void OnClickLeaveRoom()
    {
        GameManager.gm.m_lobbyPlayer.m_pv.RPC("ClearForm", RpcTarget.All);
        NetworkManager.LeaveRoom();
        UIManager.FadeInImage(m_imgDarkChange, 1f);
    }

    //로비로 돌아가겠냐는 메세지에 취소버튼
    public void OnClickReturnCancel()
    {
        UIManager.HideScale(m_imgReturnLobbyPanel.gameObject);
    }

    public void OnClickSendChat()
    {
        if (m_inputChat.text == "") return;

        GameManager.LobbyPlayerChat(m_inputChat.text);
        m_inputChat.text = "";
    }

    public void OnClickClassChange()
    {
        if (m_goClassPanel.activeSelf)
        {
            UIManager.HideScale(m_goClassPanel);
        }

        else
        {
            UIManager.ShowScale(m_goClassPanel);

            GameManager.gm.m_lobbyPlayer.ChangeForm((int)GameManager.gm.m_lobbyPlayer.m_eCurClass);
        }
    }

    public void OnClickPreviousClass()
    {
        int classIdx = (int)GameManager.gm.m_lobbyPlayer.m_eCurClass - 1;

        if (classIdx < 0) classIdx = (int)CLASSTYPE._MAX_ - 1;

        GameManager.gm.m_lobbyPlayer.ChangeForm(classIdx);
    }

    public void OnClickNextClass()
    {
        int classIdx = (int)GameManager.gm.m_lobbyPlayer.m_eCurClass + 1;

        if (classIdx >= (int)CLASSTYPE._MAX_) classIdx = 0;

        GameManager.gm.m_lobbyPlayer.ChangeForm(classIdx);
    }

    public void ChangeStatUI()
    {
        for (int i = 0; i < m_trMaxStatPoints.Length; i++)
        {
            var moveX = (m_trMaxStatPoints[i].localPosition.x / 10f) * GameManager.gm.m_lobbyPlayer.m_listStats[i];
            var moveY = (m_trMaxStatPoints[i].localPosition.y / 10f) * GameManager.gm.m_lobbyPlayer.m_listStats[i];

            m_trCurStatPoints[i].DOLocalMoveX(moveX, 0.3f);
            m_trCurStatPoints[i].DOLocalMoveY(moveY, 0.3f);
        }

        m_lineStats.positionCount = m_trCurStatPoints.Length + 1;
    }

    public void OnClickGameStart()
    {
        if (m_isStarted) return;

        m_isStarted = true;
        NetworkManager.nm.PV.RPC("GameStart", RpcTarget.All);
        //StartCoroutine(GameStart());
    }

    public IEnumerator GameStart()
    {
        //StartCoroutine(m_camMoving.MoveCam());
        //여기서 UI다 꺼줘야됨

        if (PhotonNetwork.IsMasterClient)
            UIManager.HideScale(m_txtGameStart.gameObject);

        m_goClassPanel.SetActive(false);

        cinemachineMoving.ChangeCameraTargetSmooth(m_InitFollow, m_newFollow, m_InitLookAt ,m_newLookAt, 4f, Ease.InQuart);

        yield return new WaitForSeconds(1.0f);

        UIManager.HideScale(m_txtReturnLobby.gameObject);
        UIManager.HideScale(m_goJoy);
        UIManager.HideScale(m_goChatPanel);
        UIManager.HideScale(m_imgClassButton.gameObject);
        UIManager.HideScale(m_imgSettingButton.gameObject);

        yield return new WaitForSeconds(2f);

        UIManager.FadeInImage(m_imgDarkChange, 2f);

        yield return new WaitForSeconds(2f);

        GameManager.gm.m_BattlePosIdx = 0;

        for (; GameManager.gm.m_BattlePosIdx < PhotonNetwork.PlayerList.Length; GameManager.gm.m_BattlePosIdx++)
        {
            if (PhotonNetwork.PlayerList[GameManager.gm.m_BattlePosIdx].NickName == GameManager.gm.m_lobbyPlayer.m_pv.Owner.NickName) break;
        }

        GameManager.gm.m_SavedClassIdx = GameManager.gm.m_lobbyPlayer.m_classIdx;

        PhotonNetwork.Destroy(GameManager.gm.m_lobbyPlayer.gameObject);
        UIManager.um.StartCoroutine(GameManager.ChangeScene("InGameScene", GAMESTATE.INGAME, 0f));
    }
}
