using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager udm;

    CLASSSTATS m_saveStats = new CLASSSTATS();

    private void Awake()
    {
        udm = this;
        DontDestroyOnLoad(udm);
    }

    static public void SaveUserStats(CLASSSTATS curStats)
    {
        udm.m_saveStats = curStats;
    }

    static public CLASSSTATS LoadUserStats()
    {
        return udm.m_saveStats;
    }
}
