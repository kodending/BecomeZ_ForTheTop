using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInWait : BaseState
{
    public override void OnEnterState()
    {
        GameManager.m_curState = GAMESTATE.WAITROOM;

        if(!GameManager.gm.bReturnRoom)
        {
            if (!GameManager.gm.m_bRoomCreator)
            {
                NetworkManager.JoinRoom(GameManager.gm.m_curRoomInfo.m_networkRoomInfo.Name);
            }
            else
            {
                NetworkManager.CreateRoom(GameManager.gm.m_curRoomName, GameManager.gm.m_curRoomPassword);
            }
        }

        else
        {
            GameObject lobbyPlayer = PhotonNetwork.Instantiate("LOBBYPLAYER_1", new Vector3(0, 0.012f, 0), Quaternion.Euler(0, 180, 0));
            lobbyPlayer.GetComponent<LobbyPlayerController>().InitInfo();
            lobbyPlayer.SetActive(true);
        }

        GameManager.gm.bReturnRoom = false;

        AudioManager.PlayBGM(BGM.WAITING, true);
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
