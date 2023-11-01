using Monument.World;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script that defines the movement of the crows,
 * inspecting the neighbors of the current node each
 * time they arrive at a new one.
 */

public class CrowBehaviour : Walker
{
    [SerializeField] private bool canWalkStairs = false;

    private NavNode lastVisitedNode = null;

    protected override void Start()
    {
        base.Start();
        MoveToNextNode();
    }

    private void MoveToNextNode()
    {
        // Select next node (a neighbor of the current)
        NavNode nextNode = SelectNextNode();

        isMoving = true;

        // Disable previous platform and assign the current one
        ManageRotativePlatforms(nextNode.RotativePlatform);

        // Update last visited node
        lastVisitedNode = currentNode;
        currentNode = nextNode;

        // Look at next node
        LookAtNode(nextNode);

        // Move to nect node
        StartCoroutine(MoveToNodeCoroutine(lastVisitedNode, nextNode, MoveToNextNode));
    }

    private NavNode SelectNextNode()
    {
        // Get a list of unvisited neighbors
        List<NavNode> unvisitedNeighbors = new List<NavNode>();

        foreach (NavNode neighbor in currentNode.Neighbors)
        {
            if (neighbor != lastVisitedNode)
            {
                if (neighbor.IsStairs)
                {
                    // If the crow can walk by stairs
                    if (canWalkStairs) unvisitedNeighbors.Add(neighbor);
                }
                else
                {
                    unvisitedNeighbors.Add(neighbor);
                }
            }
        }

        // If there are unvisited neighbors, choose one randomly
        if (unvisitedNeighbors.Count > 0)
        {
            int randomIndex = Random.Range(0, unvisitedNeighbors.Count);
            return unvisitedNeighbors[randomIndex];
        }
        else
        {
            // If all neighbors have been visited, return the last visited node
            return lastVisitedNode;
        }
    }
}