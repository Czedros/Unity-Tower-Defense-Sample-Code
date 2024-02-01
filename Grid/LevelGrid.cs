using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    public event EventHandler onAnyUnitMoved;

    [SerializeField] private Transform test;

    [SerializeField] private int Width;
    [SerializeField] private int Height;
    [SerializeField] private float cellSize;


    private GridSystem<GridObject> gridSystem;
    void Awake()
    {
        Instance = this;
        gridSystem = new GridSystem<GridObject>(Width, Height, cellSize, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        //gridSystem.createDebugObjectVisual(test);



    }
    private void Start()
    {
        Pathfinding.Instance.Setup(Width, Height, cellSize);
    }
    public void AddUnitAtPosition(Unit unit, GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObjectAt(gridPosition);
        gridObject.AddUnitHere(unit);
    }
    public List<Unit> GetUnitAtPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObjectAt(gridPosition);
        return gridObject.GetUnitsHere();
    }
    public void RemoveUnitAtPosition(Unit unit, GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObjectAt(gridPosition);
        gridObject.RemoveUnitHere(unit);
    }

    public void UnitChangedGridPosition(Unit unit, GridPosition fromPosition, GridPosition toPosition)
    {
        RemoveUnitAtPosition(unit, fromPosition);
        AddUnitAtPosition(unit, toPosition);

        onAnyUnitMoved?.Invoke(this, EventArgs.Empty);
    }

    public bool HasUnitOnPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObjectAt(gridPosition);
        return gridObject.HasAnyUnit();
    }
    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObjectAt(gridPosition);
        return gridObject.GetUnitHere();
    }

    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    public bool IsValidPosition(GridPosition gridPosition) => gridSystem.isValidPosition(gridPosition);



}
