using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerInLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] Text m_txtLobby;
    [SerializeField] Image m_imgDarkChange;
    [SerializeField] Text m_txtReturnText;
    [SerializeField] Image m_imgRoomInfoPanel;
    [SerializeField] Text m_txtCreateRoom;
    [SerializeField] Text m_txtRefreshRoom;

    public LobbyRoomInfo[] m_arrLobbyRooms;

    #region 방 생성패널
    [SerializeField] Image m_imgCreateRoomPanel;
    [SerializeField] InputField m_inputRoomName;
    [SerializeField] InputField m_inputPassword;
    #endregion

    #region 방 입장패널
    [SerializeField] Image m_imgJoinRoomPanel;
    [SerializeField] Text m_txtRoomName;
    [SerializeField] Image m_imgJoinPasswordPanel;
    [SerializeField] InputField m_inputJoinPassword;
    public LobbyRoomInfo m_curSelectLobbyRoomInfo;
    #endregion

    private void Start()
    {
        m_imgDarkChange.gameObject.SetActive(true);
        UIManager.FadeOutImage(m_imgDarkChange, 1f);

        StartCoroutine(OnLobbyText());
    }

    IEnumerator OnLobbyText()
    {
        yield return new WaitForSeconds(1f);

        UIManager.FadeInOutText(m_txtLobby, 1f);

        yield return new WaitForSeconds(2f);

        UIManager.ShowScale(m_imgRoomInfoPanel.gameObject, 0.3f);

        yield return new WaitForSeconds(0.5f);

        UIManager.ShowScale(m_txtReturnText.gameObject);
        UIManager.FadeInText(m_txtCreateRoom, 0.5f);
        UIManager.FadeInText(m_txtRefreshRoom, 0.5f);

        RefreshRoomInfo();
    }

    public void OnClickReturnMain()
    {
        NetworkManager.DisConnect();
        UIManager.FadeInImage(m_imgDarkChange, 1f);
    }

    public void RefreshRoomInfo()
    {
        int count = NetworkManager.nm.m_listRoomInfo.Count;
        int idx = 0;

        for (; idx < count; idx++)
        {
            var info = NetworkManager.nm.m_listRoomInfo[idx];

            bool isPublic = info.CustomProperties.ContainsKey("password") ? false : true;
            string roomName = info.Name;
            int userNum = info.PlayerCount;
            int maxNum = info.MaxPlayers;

            m_arrLobbyRooms[idx].OnRoomInfo(isPublic, roomName, userNum, maxNum);
            m_arrLobbyRooms[idx].m_networkRoomInfo = info;
        }

        for (; idx < m_arrLobbyRooms.Length; idx++)
        {
            m_arrLobbyRooms[idx].OffRoomInfo();
        }
    }

    public void OnClickRefresh()
    {
        RefreshRoomInfo();
    }

    public void OnClickOpenRoomPanel()
    {
        m_inputRoomName.text = "";
        m_inputPassword.text = "";
        UIManager.ShowScale(m_imgCreateRoomPanel.gameObject);
    }

    public void OnClickCreate()
    {
        GameManager.gm.m_curRoomName = m_inputRoomName.text;
        GameManager.gm.m_curRoomPassword = m_inputPassword.text;
        GameManager.gm.m_bRoomCreator = true;
        UIManager.FadeInImage(m_imgDarkChange, 1f);
        UIManager.um.StartCoroutine(GameManager.ChangeScene("WaitingRoomScene", GAMESTATE.WAITROOM, 1f));
    }

    public void OnClickCancel()
    {
        UIManager.HideScale(m_imgCreateRoomPanel.gameObject);
    }

    public void OnClickLobbyRoom(LobbyRoomInfo lobbyRoomInfo)
    {
        if (!lobbyRoomInfo.CheckOpen()) return;
        if (m_imgJoinRoomPanel.gameObject.activeSelf) return;

        m_curSelectLobbyRoomInfo = lobbyRoomInfo;
        m_txtRoomName.text = lobbyRoomInfo.m_networkRoomInfo.Name;

        UIManager.ShowScale(m_imgJoinRoomPanel.gameObject);
    }

    public void OnClickJoinRoom()
    {
        if(m_curSelectLobbyRoomInfo.CheckPublic())
        {
            //방 입장
            //NetworkManager.JoinRoom(m_curSelectLobbyRoomInfo.m_networkRoomInfo.Name);
            GameManager.gm.m_curRoomInfo = m_curSelectLobbyRoomInfo;
            GameManager.gm.m_bRoomCreator = false;
            UIManager.FadeInImage(m_imgDarkChange, 1f);
            UIManager.um.StartCoroutine(GameManager.ChangeScene("WaitingRoomScene", GAMESTATE.WAITROOM, 1f));
        }

        else
        {
            //비밀번호 패널 띄우기
            m_inputJoinPassword.text = "";
            UIManager.ShowScale(m_imgJoinPasswordPanel.gameObject);
        }
    }

    public void OnClickJoinCancel()
    {
        UIManager.HideScale(m_imgJoinRoomPanel.gameObject);
    }

    public void OnClickEnterPassword()
    {
        //아무튼 패널은 끄기
        UIManager.HideScale(m_imgJoinPasswordPanel.gameObject);

        //비밀번호가 일치하면 입장
        //틀렸으면 무반응
        if ((string)m_curSelectLobbyRoomInfo.m_networkRoomInfo.CustomProperties["password"] != m_inputJoinPassword.text)
        {
            UIManager.SystemMsg("비밀번호가 틀렸습니다");
            m_inputJoinPassword.text = "";
            return;
        }

        else
        {
            m_inputJoinPassword.text = "";
            //NetworkManager.JoinRoom(m_curSelectLobbyRoomInfo.m_networkRoomInfo.Name);
            GameManager.gm.m_curRoomInfo = m_curSelectLobbyRoomInfo;
            GameManager.gm.m_bRoomCreator = false;
            UIManager.FadeInImage(m_imgDarkChange, 1f);
            UIManager.um.StartCoroutine(GameManager.ChangeScene("WaitingRoomScene", GAMESTATE.WAITROOM, 1f));
            return;
        }
    }
}
