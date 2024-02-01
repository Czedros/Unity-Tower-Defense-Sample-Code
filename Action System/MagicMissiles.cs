using System;   
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MagicMissiles : BaseAction
{
    [SerializeField] private int range = 4;
    [SerializeField] private int damage = 20;
    [SerializeField] private string ActionName;
    [SerializeField] int APCost;
    private Unit targetUnit;

    public override string GetActionDescription()
    {
        return "Deal 20 damage an Unit in range";
    }

    public override string GetActionName()
    {
        return ActionName;
    }

    public override int GetAPCost()
    {
        return APCost;
    }
    public override List<GridPosition> GetValidGridPositions()
    {
        GridPosition unitPosition = unit.GetUnitPosition();
        return GetValidGridPositions(unitPosition);
    }

    public List<GridPosition> GetValidGridPositions(GridPosition unitPosition)
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                GridPosition offSetPosition = new GridPosition(x, y);
                GridPosition testGridPosition = unitPosition + offSetPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition))
                {
                    //Invalid Position
                    continue;
                }

                if(x != 0 && y != 0)
                {
                    continue;
                }

                if (testGridPosition == unitPosition)
                {
                    //Same Position
                    continue;
                }
                if (!LevelGrid.Instance.HasUnitOnPosition(testGridPosition))
                {
                    //Empty Position
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    //Same Side
                    continue;
                }

                validGridPositions.Add(testGridPosition);

            }
        }

        return validGridPositions;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        isActive = true;
    }
    private void Update()
    {
        if (!isActive) { return; }

        targetUnit.ChangeHealth(-damage, true);
        isActive = false;
        onActionComplete();

    }
    public int GetRange() => range;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 1000 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public EnemyAIAction GetEnemyEliteAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public override int GetTargetsAtPosition(GridPosition gridPosition)
    {
        return GetValidGridPositions(gridPosition).Count;
    }
}
