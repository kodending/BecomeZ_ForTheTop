using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

#region GameManager 상태머신 정의
[Tooltip("GameManager 상태머신 정의")]
public class GMStateMachine
{
    public BaseState currentState { get; private set; }
    private Dictionary<GAMESTATE, BaseState> dicStates
                                        = new Dictionary<GAMESTATE, BaseState>();
    public GMStateMachine(GAMESTATE eState, BaseState state)
    {
        AddState(eState, state);
        currentState = GetState(eState);
    }

    public void AddState(GAMESTATE eState, BaseState state)
    {
        if (!dicStates.ContainsKey(eState))
            dicStates.Add(eState, state);
    }

    public BaseState GetState(GAMESTATE eState)
    {
        if (dicStates.TryGetValue(eState, out BaseState state)) return state;

        return null;
    }

    public void DeleteState(GAMESTATE eState)
    {
        if (dicStates.ContainsKey(eState))
            dicStates.Remove(eState);
    }

    public void ChangeState(GAMESTATE eState)
    {
        currentState?.OnExitState();

        if (dicStates.TryGetValue(eState, out BaseState state))
            currentState = state;

        currentState?.OnEnterState();
    }

    public void UpdateState()
    {
        currentState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        currentState?.OnFixedUpdateState();
    }
}
#endregion

#region BattleManager 상태머신 정의
[Tooltip("BattleManager 상태머신 정의")]
public class BMStateMachine
{
    public BaseState currentState { get; private set; }
    private Dictionary<BATTLESTATE, BaseState> dicStates
                                        = new Dictionary<BATTLESTATE, BaseState>();
    public BMStateMachine(BATTLESTATE eState, BaseState state)
    {
        AddState(eState, state);
        currentState = GetState(eState);
    }

    public void AddState(BATTLESTATE eState, BaseState state)
    {
        if (!dicStates.ContainsKey(eState))
            dicStates.Add(eState, state);
    }

    public BaseState GetState(BATTLESTATE eState)
    {
        if (dicStates.TryGetValue(eState, out BaseState state)) return state;

        return null;
    }

    public void DeleteState(BATTLESTATE eState)
    {
        if (dicStates.ContainsKey(eState))
            dicStates.Remove(eState);
    }

    public void ChangeState(BATTLESTATE eState)
    {
        currentState?.OnExitState();

        if (dicStates.TryGetValue(eState, out BaseState state))
            currentState = state;

        currentState?.OnEnterState();
    }

    public void UpdateState()
    {
        currentState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        currentState?.OnFixedUpdateState();
    }
}
#endregion

#region 플레이어 상태머신 정의
[Tooltip("Player 상태머신 정의")]
public class PlayerStateMachine
{
    public PlayerBaseState currentState { get; private set; }
    private Dictionary<PLAYERSTATE, PlayerBaseState> dicStates
                                        = new Dictionary<PLAYERSTATE, PlayerBaseState>();
    public PlayerStateMachine(PLAYERSTATE eState, PlayerBaseState state)
    {
        AddState(eState, state);
        currentState = GetState(eState);
    }

    public void AddState(PLAYERSTATE eState, PlayerBaseState state)
    {
        if (!dicStates.ContainsKey(eState))
            dicStates.Add(eState, state);
    }

    public PlayerBaseState GetState(PLAYERSTATE eState)
    {
        if (dicStates.TryGetValue(eState, out PlayerBaseState state)) return state;
        return null;
    }

    public void DeleteState(PLAYERSTATE eState)
    {
        if (dicStates.ContainsKey(eState))
            dicStates.Remove(eState);
    }

    public void ChangeState(PLAYERSTATE eState)
    {
        currentState?.OnExitState();

        if (dicStates.TryGetValue(eState, out PlayerBaseState state))
            currentState = state;

        currentState?.OnEnterState();
    }

    public void UpdateState()
    {
        currentState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        currentState?.OnFixedUpdateState();
    }
}
#endregion

#region 에너미 상태머신 정의
[Tooltip("Enemy 상태머신 정의")]
public class EnemyStateMachine
{
    public EnemyBaseState currentState { get; private set; }
    private Dictionary<ENEMYSTATE, EnemyBaseState> dicStates
                                        = new Dictionary<ENEMYSTATE, EnemyBaseState>();
    public EnemyStateMachine(ENEMYSTATE eState, EnemyBaseState state)
    {
        AddState(eState, state);
        currentState = GetState(eState);
    }

    public void AddState(ENEMYSTATE eState, EnemyBaseState state)
    {
        if (!dicStates.ContainsKey(eState))
            dicStates.Add(eState, state);
    }

    public EnemyBaseState GetState(ENEMYSTATE eState)
    {
        if (dicStates.TryGetValue(eState, out EnemyBaseState state)) return state;
        return null;
    }

    public void DeleteState(ENEMYSTATE eState)
    {
        if (dicStates.ContainsKey(eState))
            dicStates.Remove(eState);
    }

    public void ChangeState(ENEMYSTATE eState)
    {
        currentState?.OnExitState();

        if (dicStates.TryGetValue(eState, out EnemyBaseState state))
            currentState = state;

        currentState?.OnEnterState();
    }

    public void UpdateState()
    {
        currentState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        currentState?.OnFixedUpdateState();
    }
}
#endregion
