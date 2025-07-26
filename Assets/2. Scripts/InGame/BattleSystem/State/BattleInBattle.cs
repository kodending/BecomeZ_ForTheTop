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

            //���⼭ �÷��̾��� ���� ������ �˾ƾߵ�
            //���Ÿ�, �и��� ���� �޶���
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

            //���⼭ ���ʹ��� �������¸� �˾ƾߵ�
            //���Ÿ�, �и�, ����� ���
            //���ʹ��� �������¸� �����ؾߵ�

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
