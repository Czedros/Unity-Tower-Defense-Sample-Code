using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private const int MOVESTRAIGHT_COST = 10;
    private const int MOVEDIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }
    private int height;
    private int width;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;
    [SerializeField] private LayerMask obstacleLayerMask;
    private void Awake()
    {
        Instance = this;
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridSystem = new GridSystem<PathNode>(width, height, cellSize, (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffset = 5f;
                if (Physics2D.Raycast(worldPosition + (Vector3.back * raycastOffset), Vector3.forward, raycastOffset * 2, obstacleLayerMask))
                {
                    GetNode(x, z).setWalkable(false);
                }
            }

        }
    }
    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition EndGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closeList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObjectAt(startGridPosition);
        PathNode endNode = gridSystem.GetGridObjectAt(EndGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObjectAt(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalcfCost();
                pathNode.ResetCamefrom();
            }

        }
        startNode.SetGCost(0);
        startNode.SetHCost(CalcDistance(startGridPosition, EndGridPosition));
        startNode.CalcfCost();

        while (openList.Count > 0)
        {

            PathNode currentNode = getLowestFNode(openList);

            if (currentNode == endNode)
            {
                pathLength = endNode.getfCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            List<PathNode> list = GetNeighbourList(currentNode);
            foreach (PathNode neighbourNode in list)
            {
                if (closeList.Contains(neighbourNode))
                {
                    continue;
                }

                if (!neighbourNode.IsWalkable())
                {
                    closeList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.getGCost() + CalcDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if (tentativeGCost < neighbourNode.getGCost())
                {
                    neighbourNode.SetCamefrom(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalcDistance(neighbourNode.GetGridPosition(), EndGridPosition));
                    neighbourNode.CalcfCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        pathLength = 0;
        return null;

    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObjectAt(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();


        if (gridPosition.x - 1 >= 0)
        {
            neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0)
            {
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }

            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }
        }
        if (gridPosition.x + 1 < gridSystem.GetWidth())
        {
            neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));

            if (gridPosition.z - 1 >= 0)
            {
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }
        }

        if (gridPosition.z - 1 >= 0)
        {
            neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
        }
        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
        }

        return neighborList;
    }

    public int CalcDistance(GridPosition a, GridPosition b)
    {
        GridPosition GridDistance = a - b;
        int xDistance = Mathf.Abs(GridDistance.x);
        int zDistance = Mathf.Abs(GridDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVEDIAGONAL_COST * Mathf.Min(xDistance, zDistance) + remaining * MOVESTRAIGHT_COST;
    }

    private PathNode getLowestFNode(List<PathNode> pathnodeList)
    {
        PathNode LowestCostNode = pathnodeList[0];
        for (int i = 0; i < pathnodeList.Count; i++)
        {
            if (pathnodeList[i].getfCost() < LowestCostNode.getfCost())
            {
                LowestCostNode = pathnodeList[i];
            }
        }
        return LowestCostNode;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodes = new List<PathNode>();
        pathNodes.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.GetCamefrom() != null)
        {
            pathNodes.Add(currentNode.GetCamefrom());
            currentNode = currentNode.GetCamefrom();
        }
        pathNodes.Reverse();
        List<GridPosition> gridPositions = new List<GridPosition>();
        foreach (PathNode node in pathNodes)
        {
            gridPositions.Add(node.GetGridPosition());
        }
        return gridPositions;

    }

    public bool IsWalkableGrid(GridPosition gridPosition)
    {
        return gridSystem.GetGridObjectAt(gridPosition).IsWalkable();
    }
    public void SetIsWalkableGrid(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObjectAt(gridPosition).setWalkable(isWalkable);
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition EndGridPosition)
    {
        return FindPath(startGridPosition, EndGridPosition, out int pathlength) != null;
    }

    public int PathLength(GridPosition startGridPosition, GridPosition EndGridPosition)
    {
        FindPath(startGridPosition, EndGridPosition, out int pathlength);
        return pathlength;
    }
}
