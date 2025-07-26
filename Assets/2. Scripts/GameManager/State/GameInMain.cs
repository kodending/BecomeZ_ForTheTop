using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInMain : BaseState
{
    public override void OnEnterState()
    {
        GameManager.m_curState = GAMESTATE.MAIN;
        GameManager.m_curUI = GameObject.Find("UIManagerInMain");

        AudioManager.PlayBGM(BGM.MAIN, true);
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
