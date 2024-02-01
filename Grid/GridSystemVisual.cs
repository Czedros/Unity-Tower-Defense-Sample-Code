using JetBrains.Annotations;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private Transform gridSystemVisualPrefab;
    private GridVisualSingle[,] gridVisualArray;
    [SerializeField] private List<Color> gridVisualList;

    private void Start()
    {
        gridVisualArray = new GridVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for(int y = 0; y < LevelGrid.Instance.GetHeight(); y++)
            {
                Transform gridVisualSingleTransform = Instantiate(gridSystemVisualPrefab, LevelGrid.Instance.GetWorldPosition(new GridPosition(x, y)), Quaternion.identity);
                gridVisualArray[x,y] = gridVisualSingleTransform.GetComponent<GridVisualSingle>();

            }
        }

        UnitActionSystem.Instance.OnSelectedActionChange += UAE_OnActionChanged;
        LevelGrid.Instance.onAnyUnitMoved += LevelGrid_onAnyUnitMoved;
        UnitActionSystem.Instance.OnSelectedUnitChange += UAE_OnActionChanged;
    }
    private void Awake()
    {
        Instance = this;
    }


    private void UAE_OnActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private void LevelGrid_onAnyUnitMoved(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridPositions()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridVisualArray[x, z].Hide();

            }

        }

    }
    public void ShowGridPositionList(List<GridPosition> gridPositions, Color color)
    {
        foreach (GridPosition gridPosition in gridPositions)
        {
            gridVisualArray[gridPosition.x, gridPosition.z].Show(color);
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPositions();

        Unit currentUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction baseAction = UnitActionSystem.Instance.GetSelectedAction();

        Color color = Color.white;

        switch (baseAction)
        {
            case MoveAction moveAction:
                color = gridVisualList[3];
                break;
            case SuicideAction spinAction:
                color = gridVisualList[2];
                break;
            case PiercingShot shotAction:
                color = gridVisualList[1];
                ShowGridPositionRangeRadial(currentUnit.GetUnitPosition(), shotAction.GetRange(), gridVisualList[4]);
                break;
            case SwordSlash slashAction:
                color = gridVisualList[1];
                ShowGridPositionRangeRadial(currentUnit.GetUnitPosition(), slashAction.GetRange(), gridVisualList[4]);
                break;
            case BuildTowerAction buildAction:
                color = gridVisualList[2];
                ShowGridPositionRangeRect(currentUnit.GetUnitPosition(), buildAction.GetRange(), gridVisualList[4]);
                break;
            case HealAction healAction:
                color = gridVisualList[2];
                ShowGridPositionRangeRect(currentUnit.GetUnitPosition(), healAction.GetRange(), gridVisualList[4]);
                break;
            case MagicMissiles missleAction:
                color = gridVisualList[1];
                ShowGridPositionRangeOrthogonal(currentUnit.GetUnitPosition(), missleAction.GetRange(), gridVisualList[4]);
                break;
            case BombToss bombAction:
                color = gridVisualList[1];
                ShowGridPositionRangeDiagonal(currentUnit.GetUnitPosition(), bombAction.GetRange(), gridVisualList[4]);
                break;
            case AOERadius radiusDamage:
                color = gridVisualList[1];
                ShowGridPositionRangeRadial(currentUnit.GetUnitPosition(), radiusDamage.GetRange(), gridVisualList[4]);
                break;

        }


        ShowGridPositionList(baseAction.GetValidGridPositions(), color);
    }
    public void ShowGridPositionRangeRadial(GridPosition gridPosition, int range, Color visualMaterial)
    {
        List<GridPosition> positions = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                GridPosition testPosition = gridPosition + new GridPosition(x, y);
                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);
                if (!LevelGrid.Instance.IsValidPosition(testPosition))
                {
                    continue;
                }
                if (testDistance > range)
                {
                    continue;
                }

                positions.Add(testPosition);
            }
        }
        ShowGridPositionList(positions, visualMaterial);
    }
    public void ShowGridPositionRangeRect(GridPosition gridPosition, int range, Color visualMaterial)
    {
        List<GridPosition> positions = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                GridPosition testPosition = gridPosition + new GridPosition(x, y);
                if (!LevelGrid.Instance.IsValidPosition(testPosition))
                {
                    continue;
                }

                positions.Add(testPosition);
            }
        }
        ShowGridPositionList(positions, visualMaterial);
    }

    public void ShowGridPositionRangeOrthogonal(GridPosition gridPosition, int range, Color visualMaterial)
    {
        List<GridPosition> positions = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                GridPosition testPosition = gridPosition + new GridPosition(x, y);
                if (!LevelGrid.Instance.IsValidPosition(testPosition))
                {
                    continue;
                }
                if (x != 0 && y != 0)
                {
                    continue;
                }
                positions.Add(testPosition);
            }
        }
        ShowGridPositionList(positions, visualMaterial);
    }
    public void ShowGridPositionRangeDiagonal(GridPosition gridPosition, int range, Color visualMaterial)
    {
        List<GridPosition> positions = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                GridPosition testPosition = gridPosition + new GridPosition(x, y);
                if (!LevelGrid.Instance.IsValidPosition(testPosition))
                {
                    continue;
                }
                if (x != y && x != -y && -x != y)
                {
                    continue;
                }

                positions.Add(testPosition);
            }
        }
        ShowGridPositionList(positions, visualMaterial);
    }

}
