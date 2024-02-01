using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniMoveAction : BaseAction
{
    [SerializeField] private int maxMoveDistance;
    [SerializeField] private string ActionName;
    [SerializeField] int APCost;
    private List<Vector3> MovePos;
    private int currentPosIndex;

    protected override void Awake()
    {
        base.Awake();
    }
    private void Update()
    {
        if (!isActive) { return; }

        Vector3 targetPosition = MovePos[currentPosIndex];

        float stopDistance = 0.02f;
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

                validGridPositions.Add(testGridPosition);

            }
        }

        return validGridPositions;
    }

    public override string GetActionName()
    {
        return ActionName;
    }

    public override string GetActionDescription()
    {
        return "This Unit moves to selected Square";
    }

    public override int GetAPCost()
    {
        return APCost;
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCount = unit.GetAction<PiercingShot>().GetTargetsAtPosition(gridPosition);
        GridPosition gridPos = LevelGrid.Instance.GetGridPosition(new Vector3(24, 24)) - gridPosition;
        int DistanceFromNexus = Mathf.Abs(gridPos.x) + Mathf.Abs(gridPos.z);
        int score = targetCount * 15 - DistanceFromNexus;
        Debug.Log("NexusModifier at " + gridPosition + ": " + (-DistanceFromNexus));
        Debug.Log("Final Score at " + gridPosition + ": " + score);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCount * 15 - DistanceFromNexus,
        };
    }

    public override int GetTargetsAtPosition(GridPosition gridPosition)
    {
        return GetValidGridPositions().Count;
    }
}
