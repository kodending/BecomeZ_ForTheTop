using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleInBattle : BaseState
{
    public override void OnEnterState()
    {
        if (BattleManager.m_listBattleTimelineInfo[0].bUser)
        {
            var player = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>();

            player.m_stateMachine.ChangeState(PLAYERSTATE.ATTACK_MOVE);
        }

        else
        {
            var enemy = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<EnemyFSM>();

            enemy.m_stateMachine.ChangeState(ENEMYSTATE.ATTACK_MOVE);
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
