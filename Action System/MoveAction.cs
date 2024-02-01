using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class MoveAction : BaseAction
{
    [SerializeField] private int maxMoveDistance;
    private List<Vector3> MovePos;
    private int currentPosIndex;

    protected override void Awake()
    {
        base.Awake();
    }
    private void FixedUpdate()
    {
        if (!isActive) { return;  }

        Vector3 targetPosition = MovePos[currentPosIndex];

        float stopDistance = 0.05f;
        if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += moveDir * Time.deltaTime * moveSpeed;
        }
        else
        {
            currentPosIndex++;
            if (currentPosIndex >= MovePos.Count)
            {
                isActive = false;
                onActionComplete();
            }
        }


    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> PathGridPositions = Pathfinding.Instance.FindPath(unit.GetUnitPosition(), gridPosition, out int length);

        currentPosIndex = 0;
        MovePos = new List<Vector3>();

        foreach (GridPosition pos in PathGridPositions)
        {
            MovePos.Add(LevelGrid.Instance.GetWorldPosition(pos));
        }
        this.onActionComplete = onActionComplete;
        isActive = true;
    }

    public override List<GridPosition> GetValidGridPositions()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();

        GridPosition unitPosition = unit.GetUnitPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int y = -maxMoveDistance; y <= maxMoveDistance; y++)
            {
                GridPosition offSetPosition = new GridPosition(x, y);
                GridPosition testGridPosition = unitPosition + offSetPosition;
               

                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);

                if (testDistance > maxMoveDistance)
                {
                    //Too far 
                    continue;
                }

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition))
                {
                    //Invalid Position
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
                if (!Pathfinding.Instance.HasPath(unitPosition, testGridPosition))
                {
                    continue;
                }
                int pathFindingMultiplier = 10;
                if (Pathfinding.Instance.PathLength(unitPosition, testGridPosition) > maxMoveDistance * pathFindingMultiplier)
                {
                    continue;
                }

                validGridPositions.Add(testGridPosition);

            }
        }

        return validGridPositions;
    }

    public override string GetActionName()
    {
        return "Move [1AP]";
    }

    public override string GetActionDescription()
    {
        return "This Unit moves to selected Square";
    }

    public override int GetAPCost()
    {
        return 1;
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCount = 0;
        foreach(BaseAction action in unit.GetBaseActions())
        {
            if (action.GetTargetsAtPosition(gridPosition) > targetCount)
            {
                targetCount= action.GetTargetsAtPosition(gridPosition);
            }
        }
        GridPosition gridPos = LevelGrid.Instance.GetGridPosition(new Vector3(24, 24)) - gridPosition;
        int DistanceFromNexus = (Mathf.Abs(gridPos.x) + Mathf.Abs(gridPos.z))/3;
        if(targetCount < 2)
        {
            DistanceFromNexus = 0;
        }
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCount * 10 - DistanceFromNexus,
        };
    }

    public override int GetTargetsAtPosition(GridPosition gridPosition)
    {
        return GetValidGridPositions().Count;
    }
}
