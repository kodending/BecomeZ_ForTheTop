using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

#region BaseState ����

public abstract class BaseState : MonoBehaviourPunCallbacks
{
    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}

#endregion

#region �÷��̾� ��������
public abstract class PlayerBaseState : MonoBehaviourPunCallbacks
{
    protected InGamePlayerController playerController { get; private set; }
    public PlayerBaseState(InGamePlayerController pc)
    {
        this.playerController = pc;
    }

    public abstract void OnEnterState();

    public abstract void OnAnimatorMove();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}
#endregion

#region ���ʹ� ��������
public abstract class EnemyBaseState : MonoBehaviourPunCallbacks
{
    protected EnemyFSM enemyFSM { get; private set; }
    public EnemyBaseState(EnemyFSM enemy)
    {
        this.enemyFSM = enemy;
    }

    public abstract void OnEnterState();
    public abstract void OnAnimatorMove();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}
#endregion
