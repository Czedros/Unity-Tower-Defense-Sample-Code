using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private int width;
    private int height;
    private float cellsize;
    private TGridObject[,] gridObjects;
    public GridSystem(int width, int height, float cellsize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellsize = cellsize;
        gridObjects = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x,y);
                gridObjects[x, y] = createGridObject(this, gridPosition);
            }

        }

    }

    public void createDebugObjectVisual(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebug gridDebugtool =  debugTransform.GetComponent<GridDebug>();
                gridDebugtool.SetGridObject(GetGridObjectAt(gridPosition) as GridObject);

            }

        }
    }

    public int GetWidth() => width;
    public int GetHeight() => height;
    public TGridObject GetGridObjectAt(GridPosition gridPosition)
    {
        return gridObjects[gridPosition.x, gridPosition.z];
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => new Vector3(gridPosition.x, gridPosition.z, 0) * cellsize;

    public GridPosition GetGridPosition(Vector3 worldPosition) => new GridPosition(Mathf.RoundToInt(worldPosition.x / cellsize), Mathf.RoundToInt(worldPosition.y / cellsize));

    public bool isValidPosition(GridPosition gridPosition) => gridPosition.x >= 0 && gridPosition.z >= 0 && gridPosition.x < width && gridPosition.z < height;
}
