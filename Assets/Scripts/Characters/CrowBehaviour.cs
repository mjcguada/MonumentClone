using Monument.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowBehaviour : Walker
{
    // TODO: stop if the crow encounters Ida
    // TODO: state machine

    [SerializeField] private bool canWalkStairs = false;

    private enum CrowStates { Walking, Stopped };

    private bool automaticNavigation = true; // if false, we have to define the available navigation nodes for the crow

    private CrowStates state = CrowStates.Walking;

    private System.Action OnMovementInterrupted;

    private NavNode lastVisitedNode = null;

    private void Awake()
    {
        // OnMovementInterrupted = FindNewPath;
        // todo: remember to use the isMoving attribute
    }

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

        // Rotative platforms management
        // Enable again rotative platform that we leave
        if (lastRotativePlatform != null)
        {
            // Handle rotation
            lastRotativePlatform.RotatorHandle?.EnableRotation(true);

            // Platform rotation
            lastRotativePlatform.EnableRotation(true);

            transform.SetParent(null);
        }

        lastRotativePlatform = nextNode.RotativePlatform;

        // Disable rotation of the current rotative platform
        if (lastRotativePlatform != null)
        {
            // Disable Handle rotation
            lastRotativePlatform.RotatorHandle?.EnableRotation(false);

            // Disable Platform rotation (while the crow is moving)
            lastRotativePlatform.EnableRotation(false);

            // Make crow child of rotative platform to rotate with it
            transform.SetParent(lastRotativePlatform.transform, true);
        }

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
