using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInLoad : BaseState
{
    public override void OnEnterState()
    {
        GameManager.m_curState = GAMESTATE.LOAD;

        StartCoroutine(GameManager.ChangeScene("MainScene", GAMESTATE.MAIN, 3f));
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
