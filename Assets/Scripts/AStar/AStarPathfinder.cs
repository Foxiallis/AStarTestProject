using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    public Camera mainCamera; //instead of Camera.main for efficiency
    public LayerMask walkableMask;
    public AStarGrid grid;

    private Vector3 currentStartingPoint;
    private Vector3 currentTarget;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && PartyManager.instance.interactable)
        {
            RaycastFromScreen();   
        }
    }

    private void RaycastFromScreen()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, walkableMask))
        {
            currentStartingPoint = PartyManager.instance.partyLeader.transform.position;
            currentTarget = hit.point;
            List<AStarNode> path = FindPath(currentStartingPoint, currentTarget);
            if (path.Count > 0)
            {
                PartyManager.instance.StartMoving();
                PartyManager.instance.SetPath(path);
            }
        }
    }

    private List<AStarNode> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        AStarNode startNode = grid.NodeFromWorldPoint(startPos);
        AStarNode targetNode = grid.NodeFromWorldPoint(targetPos);

        List<AStarNode> openSet = new List<AStarNode>();
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            AStarNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (AStarNode neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parentNode = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return new List<AStarNode>();
    }

    public void RecalculateNodes()
    {
        if (!grid.UpdateNodes()) return;

        PartyManager.instance.SetPath(FindPath(PartyManager.instance.partyLeader.transform.position, currentTarget));
    }

    private List<AStarNode> RetracePath(AStarNode startNode, AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();

        return path;
    }

    private int GetDistance(AStarNode nodeA, AStarNode nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridPositionX - nodeB.gridPositionX);
        int distY = Mathf.Abs(nodeA.gridPositionY - nodeB.gridPositionY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}