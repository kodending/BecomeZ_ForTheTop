using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInLobby : BaseState
{
    public override void OnEnterState()
    {
        GameManager.m_curState = GAMESTATE.LOBBY;
        GameManager.m_curUI = GameObject.Find("UIManagerInLobby");


        AudioManager.PlayBGM(BGM.LOBBY, true);
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
