using Monument.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Walker : MonoBehaviour, IPresser
{
    // Pathfinding variables
    protected bool isMoving = false;

    // Translation time between nodes
    [Range(0.1f, 1f)]
    [SerializeField] protected float timeToArrive = 0.25f;

    protected NavNode currentNode;

    // The last rotative platform walked by the character
    protected RotativePlatform lastRotativePlatform;

    protected virtual void Start()
    {
        currentNode = FindNodeUnderCharacter();
        lastRotativePlatform = currentNode.RotativePlatform;

        if (lastRotativePlatform != null) gameObject.transform.SetParent(lastRotativePlatform.transform);
    }

    protected NavNode FindNodeUnderCharacter()
    {
        // Raycast to find a Node under the Walker
        Collider[] colliders = Physics.OverlapSphere(transform.position - transform.TransformDirection(Vector3.up) * 0.25f, 0.2f);

        // Filter colliders to keep only those with NavNode component
        List<Collider> collidersWithNavNode = colliders.Where(collider => collider.gameObject.GetComponent<NavNode>() != null).ToList();

        // Find closest collider
        int index = 0;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < collidersWithNavNode.Count; i++)
        {
            float colliderDistance = Vector3.Distance(transform.position, collidersWithNavNode[i].transform.position);
            if (colliderDistance < minDistance)
            {
                minDistance = colliderDistance;
                index = i;
            }
        }

        return collidersWithNavNode[index].gameObject.GetComponent<NavNode>();
    }

    // Disable previous platform and assign the current one
    protected void ManageRotativePlatforms(RotativePlatform currentPlatform) 
    {
        // Enable again rotative platform that we leave
        if (lastRotativePlatform != null)
        {
            // Handle rotation
            lastRotativePlatform.RotatorHandle?.EnableRotation(true);

            // Platform rotation
            lastRotativePlatform.EnableRotation(true);

            transform.SetParent(null);
        }

        lastRotativePlatform = currentPlatform;

        // Disable rotation of the current rotative platform
        if (lastRotativePlatform != null)
        {
            // Disable Handle rotation
            lastRotativePlatform.RotatorHandle?.EnableRotation(false);

            // Disable Platform rotation (while the crow is moving)
            lastRotativePlatform.EnableRotation(false);

            // Make the character child of the rotative platform to rotate with it
            transform.SetParent(lastRotativePlatform.transform, true);
        }
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

    protected IEnumerator MoveToNodeCoroutine(NavNode startingNode, NavNode targetNode, System.Action OnMovementFinished)
    {
        isMoving = true;

        float elapsedTime = 0;
        while (isMoving && elapsedTime < timeToArrive)
        {
            // Movement
            transform.position = Vector3.Lerp(startingNode.WalkPoint, targetNode.WalkPoint, elapsedTime / timeToArrive);
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        isMoving = false;

        currentNode = targetNode;

        OnMovementFinished?.Invoke();
    }

    public void StopMovement()
    {
        StopAllCoroutines();
        isMoving = false;
    }
}
