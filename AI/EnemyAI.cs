using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
       WaitingForTurn,
       TakingTurn,
       Busy,
    }

    private State state;

    private float timer;
    private void Start()
    {
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChanged;
    }
    private void Awake()
    {
        state = State.WaitingForTurn;
    }


    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return; 
        }
        switch (state)
        {
            case State.WaitingForTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAction(SetStateTakeTurn))
                    {
                        state = State.Busy;
                        timer = 3f;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = State.TakingTurn;
                }
                break;
        }
    }

    private void SetStateTakeTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if(!TurnSystem.Instance.IsPlayerTurn())
        {
            SetStateTakeTurn();
        }
        else
        {
            state = State.WaitingForTurn;
        }
    }

    private bool TryTakeEnemyAction(Action onEnemyAIActionComplete)
    {
        List<Unit> enemyList = UnitManager.Instance.GetEnemyUnits();
        foreach (Unit enemy in enemyList)
        {
            if (TryTakeAIAction(enemy, onEnemyAIActionComplete))
            {
                return true;
            }
        }
        return false;
    }
    private bool TryTakeAIAction(Unit enemy, Action onActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;
        foreach (BaseAction baseAction in enemy.GetBaseActions())
        {

            if (!enemy.CanSpendAPtoTakeAction(baseAction))
            {
                continue;
            }
            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestEnemyAIAction != null && enemy.TrySpendAPtoTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }
}
