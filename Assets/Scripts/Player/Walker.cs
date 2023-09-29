using Monument.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Walker : MonoBehaviour
{
    // Pathfinding variables
    protected bool isMoving = false;

    // Translation time between nodes
    [Range(0.1f, 1f)]
    [SerializeField] protected float timeToArrive = 0.25f;

    protected NavNode currentNode;

    protected virtual void Start()
    {
        currentNode = FindNodeUnderPlayer();
    }

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

    protected IEnumerator MoveToNodeCoroutine(NavNode targetNode, System.Action OnMovementFinished)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        Vector3 targetPosition = targetNode.WalkPoint;

        while (elapsedTime < timeToArrive)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / timeToArrive);
            yield return null;
        }

        OnMovementFinished?.Invoke();
    }
}
