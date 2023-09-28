using Monument.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Walker : MonoBehaviour
{
    // Pathfinding variables
    protected bool isMoving = false;
    protected List<NavNode> pathToFollow = new List<NavNode>();

    // Translation time between nodes
    [Range(0.1f, 1f)]
    [SerializeField] protected float timeToArrive = 0.25f;

    protected System.Action OnMovementInterrupted;

    protected NavNode FindNodeUnderPlayer()
    {
        // Raycast to find origin Node
        Collider[] colliders = Physics.OverlapSphere(transform.position - transform.up * 0.5f, 0.2f);

        return colliders[0].gameObject.GetComponent<NavNode>();
    }

    protected void LookAtNode(NavNode targetNode)
    {
        // Convert Node world coordinates to screen space
        Vector3 nodeScreenPosition = Camera.main.WorldToScreenPoint(targetNode.WalkPoint);

        // convert Node screen point to Ray
        Ray rayToNextPosition = Camera.main.ScreenPointToRay(nodeScreenPosition);

        //We create a plane at the player's feet to intersect the ray and calculate the angle between them
        Plane plane = new Plane(Vector3.up, transform.position);

        // Project the Node onto the plane and face towards projected point
        if (plane.Raycast(rayToNextPosition, out float cameraDistance))
        {
            Vector3 nextPositionOnPlane = rayToNextPosition.GetPoint(cameraDistance);
            Vector3 playerPosition = transform.position;

            Vector3 directionToNextNode = nextPositionOnPlane - playerPosition;
            if (directionToNextNode != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionToNextNode);
            }
        }
    }

    // TODO: Crow and player will have different implementations
    protected IEnumerator MoveToNodeCoroutine(int currentIndex)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        Vector3 targetPosition = pathToFollow[currentIndex].WalkPoint;

        while (elapsedTime < timeToArrive)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / timeToArrive);
            yield return null;
        }

        int nextIndex = currentIndex + 1;

        // if the crow has reached the end of the path
        if (nextIndex >= pathToFollow.Count)
        {
            OnMovementInterrupted?.Invoke();
            yield break;
        }

        // Before moving to the next Node, we have to check if it's still an active neighbor
        if (!pathToFollow[currentIndex].Neighbors.Contains(pathToFollow[nextIndex]))
        {
            OnMovementInterrupted?.Invoke();
            yield break;
        }

        // Move to next index of the path
        MoveTo(nextIndex);
    }

    protected abstract void MoveTo(int currentIndex);
}
