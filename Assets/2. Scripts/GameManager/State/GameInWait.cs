using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInWait : BaseState
{
    public override void OnEnterState()
    {
        GameManager.m_curState = GAMESTATE.WAITROOM;

        if(!GameManager.gm.m_bRoomCreator)
        {
            NetworkManager.JoinRoom(GameManager.gm.m_curRoomInfo.m_networkRoomInfo.Name);
        }
        else
        {
            NetworkManager.CreateRoom(GameManager.gm.m_curRoomName, GameManager.gm.m_curRoomPassword);
        }
    }

    public override void OnUpdateState()
    {

    }

    public override void OnFixedUpdateState()
    {

    }

    public override void OnExitState()
    {

    }
}
