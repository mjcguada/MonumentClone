using Monument.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowBehaviour : Walker
{
    // TODO: stop if the crow encounters Ida
    // TODO: find a new path if encounter another Crow
        // - Maybe raycast or a state machine

    private void Awake()
    {
        OnMovementInterrupted = FindNewPath;
    }

    void Start()
    {
        FindNewPath();
    }

    // Searches for the longest path available (usually the opposite node)
    private void FindNewPath() 
    {
        pathToFollow = NavNodePathFinder.FindLongestPathFrom(FindNodeUnderPlayer());
        MoveTo(currentIndex: 1); // index 0 is the origin node
    }

    protected override void MoveTo(int currentIndex)
    {
        // if the given index is smaller than the length of the list
        // we continue moving
        if (currentIndex < pathToFollow.Count)
        {
            isMoving = true;

            // Look at next node
            LookAtNode(pathToFollow[currentIndex]);

            // Move to next node
            StartCoroutine(MoveToNodeCoroutine(currentIndex));
        }
    }    

}
