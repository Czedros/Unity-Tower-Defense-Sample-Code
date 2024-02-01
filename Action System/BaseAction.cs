using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract string GetActionDescription();
    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual bool isValidPosition(GridPosition gridPosition)
    {
        List<GridPosition> validPositions = GetValidGridPositions();
        return validPositions.Contains(gridPosition);
    }
    public abstract List<GridPosition> GetValidGridPositions();

    public abstract int GetAPCost();

    public EnemyAIAction GetBestAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();
        List<GridPosition> validActionGridPositionList = GetValidGridPositions();

        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0];
        }
        else
        {
            return null;
        }
    }
    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
    public abstract int GetTargetsAtPosition(GridPosition gridPosition);
}

