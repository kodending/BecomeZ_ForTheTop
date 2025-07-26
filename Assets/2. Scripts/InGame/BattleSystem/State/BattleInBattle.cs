using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleInBattle : BaseState
{
    SKILLINFO curSkillInfo = new SKILLINFO();

    public override void OnEnterState()
    {
        if (BattleManager.m_listBattleTimelineInfo[0].bUser)
        {
            var player = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<InGamePlayerController>();

            //여기서 플레이어의 공격 유형을 알아야됨
            //원거리, 밀리에 따라 달라짐
            curSkillInfo = player.m_sCurStats.listSkills[BattleManager.m_iCurSelectedSkill];
            switch ((SKILLTYPE)curSkillInfo.iSkillType)
            {
                case SKILLTYPE.MELEE_ATTACk:
                    player.m_stateMachine.ChangeState(PLAYERSTATE.ATTACK_MOVE);
                    break;
                case SKILLTYPE.RANGED_ATTACK:
                    player.m_stateMachine.ChangeState(PLAYERSTATE.RANGED_ATTACk);
                    break;
            }
        }

        else
        {
            var enemy = BattleManager.m_listBattleTimelineInfo[0].go.GetComponent<EnemyFSM>();

            //여기서 에너미의 공격형태를 알아야됨
            //원거리, 밀리, 스페셜 등등
            //에너미의 공격형태를 결정해야됨

            switch ((SKILLTYPE)enemy.m_sSelectedSkill.iSkillType)
            {
                case SKILLTYPE.MELEE_ATTACk:
                    enemy.m_stateMachine.ChangeState(ENEMYSTATE.ATTACK_MOVE);
                    break;
                case SKILLTYPE.RANGED_ATTACK:
                    enemy.m_stateMachine.ChangeState(ENEMYSTATE.RANGED_ATTACK);
                    break;
            }
            
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
