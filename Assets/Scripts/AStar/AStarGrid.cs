//#define DEBUG_GRID //Value for debug purposes such as displaying nodes, comment to disable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public AStarNode[,] grid;

#if DEBUG_GRID
    [Header("Debug")]
    public GameObject walkableNodePrefab;
    public GameObject unwalkableNodePrefab;
#endif

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    private Vector3 worldBottomLeft;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new AStarNode[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                CreateNode(x, y);
            }
        }
    }

    private void CreateNode(int x, int y)
    {
        Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
        bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
        grid[x, y] = new AStarNode(walkable, worldPoint, x, y);

#if DEBUG_GRID
        InstantiateDebugNode(walkable, worldPoint);
#endif
    }

#if DEBUG_GRID
    private void InstantiateDebugNode(bool walkable, Vector3 worldPoint) //Places a colored node depending if the place is walkable or not
    {
        if (walkable)
        {
            Instantiate(walkableNodePrefab, worldPoint, Quaternion.identity);
        }
        else
        {
            Instantiate(unwalkableNodePrefab, worldPoint, Quaternion.identity);
        }
    }
#endif

    public bool UpdateNodes()
    {
        bool atLeastOneUpdate = false;
        foreach (AStarNode node in grid)
        {
            bool newWalkable = !Physics.CheckSphere(node.worldPosition, nodeRadius, unwalkableMask);
            if (node.walkable != newWalkable) atLeastOneUpdate = true;
            node.walkable = newWalkable;
        }
        return atLeastOneUpdate;
    }

    public AStarNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<AStarNode> GetNeighbours(AStarNode node)
    {
        List<AStarNode> neighbours = new List<AStarNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridPositionX + x;
                int checkY = node.gridPositionY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
}
