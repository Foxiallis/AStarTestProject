using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridPositionX;
    public int gridPositionY;
    public int gCost;
    public int hCost;
    public AStarNode parentNode;

    public AStarNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridPositionX = _gridX;
        gridPositionY = _gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}