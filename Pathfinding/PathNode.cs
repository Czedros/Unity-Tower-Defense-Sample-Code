using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition gridPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private PathNode previousNode;
    private bool isWalkable = true;
    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }
    public override string ToString()
    {
        return gridPosition.ToString();
    }
    public int getGCost()
    {
        return gCost;
    }
    public int getHCost()
    {
        return hCost;
    }
    public int getfCost()
    {
        return fCost;
    }
    public void SetGCost(int cost)
    {
        gCost = cost;
    }
    public void SetHCost(int cost)
    {
        hCost = cost;
    }
    public void CalcfCost()
    {
        fCost = gCost + hCost;
    }
    public void ResetCamefrom()
    {
        previousNode = null;
    }
    public void SetCamefrom(PathNode path)
    {
        previousNode = path;
    }
    public PathNode GetCamefrom()
    {
        return previousNode;
    }
    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }
    public void setWalkable(bool walkable)
    {
        isWalkable = walkable;
    }
}