using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using PlayFab.GroupsModels;

public class UserDataManager : MonoBehaviourPunCallbacks
{
    public static UserDataManager udm;
    public static PhotonView PV;

    static CLASSSTATS[] m_arrsSyncUserInfo = new CLASSSTATS[3];

    private void Awake()
    {
        udm = this;
        DontDestroyOnLoad(udm);
    }

    static public void SaveMyUserInfo(int userIdx, CLASSSTATS sInfo)
    {
        m_arrsSyncUserInfo[userIdx] = sInfo;
    }

    static public CLASSSTATS LoadMyUserInfo(string userName)
    {
        CLASSSTATS sInfo = new CLASSSTATS();

        bool bCheck = Array.Exists(m_arrsSyncUserInfo, x => x.name == userName);

        if (bCheck)
        {
            int idx = Array.FindIndex(m_arrsSyncUserInfo, x => x.name == userName);

            return m_arrsSyncUserInfo[idx];
        }

        else
        {
            return sInfo;
        }
    }

    static public void ClearMyUserInfo(int userIdx)
    {
        CLASSSTATS sInfo = new();

        m_arrsSyncUserInfo[userIdx] = sInfo;
    }
}
