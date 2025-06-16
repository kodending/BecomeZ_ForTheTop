using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomInfo : MonoBehaviourPunCallbacks
{
    [Header("방 정보들")]
    [SerializeField] GameObject m_goOutline;
    [SerializeField] GameObject m_goPublicLock;
    [SerializeField] GameObject m_goPrivateLock;
    [SerializeField] Text m_txtRoom;
    [SerializeField] Text m_txtUserInfo;
    public RoomInfo m_networkRoomInfo;
    bool m_isOpen;

    public void OnRoomInfo(bool isPublic, string roomName, int userNum, int maxNum)
    {
        m_isOpen = true;
        m_goOutline.SetActive(true);

        if(isPublic) m_goPublicLock.SetActive(true);
        else m_goPrivateLock.SetActive(true);

        m_txtRoom.gameObject.SetActive(true);

        m_txtRoom.text = roomName;

        m_txtUserInfo.gameObject.SetActive(true);

        string strUser = userNum.ToString() + " / " + maxNum.ToString();

        m_txtUserInfo.text = strUser;
    }

    public void OffRoomInfo()
    {
        m_isOpen = false;

        m_networkRoomInfo = null;
        m_goOutline.SetActive(false);
        m_goPublicLock.SetActive(false);
        m_goPrivateLock.SetActive(false);
        m_txtRoom.gameObject.SetActive(false);
        m_txtUserInfo.gameObject.SetActive(false);
    }

    public bool CheckOpen()
    {
        return m_isOpen;
    }

    public bool CheckPublic()
    {
        if (!m_networkRoomInfo.CustomProperties.ContainsKey("password")) return true;
        else return false;
    }
}
