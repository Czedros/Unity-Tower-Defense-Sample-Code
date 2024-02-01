using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapStrike : BaseAction
{
    [SerializeField] private int range = 1;
    [SerializeField] private int Hitrange = 1;
    [SerializeField] private int damage = 15;
    [SerializeField] string ActionName;
    [SerializeField] int APCost;
    [SerializeField] bool IsMagic;
    private Vector3 targetPosition;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        List<Unit> targets = new List<Unit>();
        isActive = true;
        targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
;
    }

    private void Update()
    {
        if (!isActive) { return; }

        float stopDistance = 0.04f;
        if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;
            float moveSpeed = 8f;
            transform.position += moveDir * Time.deltaTime * moveSpeed;
        }
        else
        {
            foreach (Unit target in ValidUnitsInHitRadius(LevelGrid.Instance.GetGridPosition(targetPosition)))
            {
                target.ChangeHealth(-damage, IsMagic);

            }
            isActive = false;
            onActionComplete();
        }
    }

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

                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);

                if (testDistance > range)
                {
                    continue;
                }

                if (testGridPosition == unitPosition)
                {
                    //Same Position
                    continue;
                }

                if (LevelGrid.Instance.HasUnitOnPosition(testGridPosition))
                {
                    //Occupied Position
                    continue;
                }
                if (!Pathfinding.Instance.IsWalkableGrid(testGridPosition))
                {
                    continue;
                }
                
                validGridPositions.Add(testGridPosition);

            }
        }

        return validGridPositions;
    }
    
    private List<Unit> ValidUnitsInHitRadius(GridPosition gridPosition)
    {
        List<Unit> HitUnits = new List<Unit>();

        for (int x = -Hitrange; x <= Hitrange; x++)
        {
            for (int y = -Hitrange; y <= Hitrange; y++)
            {
                GridPosition offSetPosition = new GridPosition(x, y);
                GridPosition testGridPosition = gridPosition + offSetPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition))
                {
                    //Invalid Position
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);

                if (testDistance > Hitrange)
                {
                    continue;
                }

                if (testGridPosition == gridPosition)
                {
                    //Same Position
                    continue;
                }

                if (!LevelGrid.Instance.HasUnitOnPosition(testGridPosition))
                {
                    //Occupied Position
                    continue;
                }
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    //Same Side
                    continue;
                }

                HitUnits.Add(targetUnit);

            }
        }

        return HitUnits;
    }

    public int GetRange() => range;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
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