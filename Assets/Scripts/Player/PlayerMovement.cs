using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monument.World;
using static UnityEngine.GraphicsBuffer;

namespace Monument.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerAnimator animator;
        [SerializeField] private PlayerCursor cursor;

        private MonumentInput inputActions;

        // The last rotative platform used by the player
        private Rotable lastRotativePlatform;

        // Pathfinding variables
        private bool isMoving = false;
        private Coroutine findPathCoroutine = null;
        List<NavNode> pathToFollow = new List<NavNode>();

        // Translation time between nodes
        private const float timeToArrive = 0.25f;

        void Start()
        {
            inputActions = new MonumentInput();
            inputActions.Player.Click.performed += ctx => OnClick();
            inputActions.Enable();
        }

        private void OnDisable()
        {
            if (inputActions != null) inputActions.Disable();
        }

        private void OnEnable()
        {
            if (inputActions != null) inputActions.Enable();
        }

        private void OnClick()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // If the player has touch a navegable node
                // We create a new path to follow
                NavNode target = hit.transform.gameObject.GetComponent<NavNode>();
                if (target != null)
                {
                    if (findPathCoroutine != null) StopCoroutine(findPathCoroutine);
                    findPathCoroutine = StartCoroutine(FindPathToTarget(target));
                }
            }
        }

        private IEnumerator FindPathToTarget(NavNode target)
        {
            // Clear existing path
            pathToFollow.Clear();

            // Show cursor
            cursor?.ShowCursor(Camera.main.WorldToScreenPoint(target.WalkPoint));

            // Wait until the player reaches the Node they were moving to
            while (isMoving)
            {
                yield return null;
            }

            // Raycast to find origin Node
            Collider[] colliders = Physics.OverlapSphere(transform.position - transform.up * 0.5f, 0.2f);

            NavNode originNode = colliders[0].gameObject.GetComponent<NavNode>();

            if (originNode == null)
            {
                Debug.LogError("Couldn't find origin node");
                yield break;
            }
            else if (originNode == target)
            {
                Debug.Log("Target and origin nodes are the same one");
                yield break;
            }

            Queue<NavNode> queue = new Queue<NavNode>();
            queue.Enqueue(originNode);

            Dictionary<NavNode, NavNode> parentMap = new Dictionary<NavNode, NavNode>();
            parentMap[originNode] = null;

            bool pathFound = false;

            // Inspect every neighbor and their children recursively
            while (queue.Count > 0)
            {
                NavNode current = queue.Dequeue();

                if (current == target)
                {
                    pathFound = true;
                    break;
                }

                foreach (Neighbor neighbor in current.Neighbors)
                {
                    if (!neighbor.isActive) continue;

                    if (!parentMap.ContainsKey(neighbor.Node))
                    {
                        queue.Enqueue(neighbor.Node);
                        parentMap[neighbor.Node] = current;
                    }
                }
            }

            if (pathFound)
            {
                List<NavNode> path = BuildPathFromParentMap(parentMap, target);
                FollowPath(path);
            }
            else
            {
                Debug.Log("Path not found.");
            }
        }

        private List<NavNode> BuildPathFromParentMap(Dictionary<NavNode, NavNode> parentMap, NavNode target)
        {
            List<NavNode> path = new List<NavNode>();
            NavNode current = target;

            while (current != null)
            {
                path.Insert(0, current);
                current = parentMap[current];
            }

            return path;
        }

        private void FollowPath(List<NavNode> path)
        {
            animator.Walking(true);

            pathToFollow = path;
            MoveTo(currentIndex: 1); // index 0 is the origin node
        }

        private void MoveTo(int currentIndex)
        {
            // if the given index is smaller than the length of the list
            // we continue moving
            if (currentIndex < pathToFollow.Count)
            {
                isMoving = true;

                if (lastRotativePlatform != null) lastRotativePlatform.AllowsRotation = true;

                if (pathToFollow[currentIndex].RotativePlatform != null)
                {
                    lastRotativePlatform = pathToFollow[currentIndex].RotativePlatform;
                    lastRotativePlatform.AllowsRotation = false;
                }

                // Look at next node
                LookAtNode(pathToFollow[currentIndex]);

                // Move to next node
                StartCoroutine(MoveToPosition(currentIndex));
            }
            else
            {
                isMoving = false;
            }
        }

        IEnumerator MoveToPosition(int currentIndex)
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

            // if the players have changed the currentPath or if they have reached the end of it
            if (nextIndex >= pathToFollow.Count)
            {
                isMoving = false;
                animator.Walking(false);
                yield break;
            }

            // Before moving to the next Node, we have to check if it's still an active neighbor (something could have changed)
            if (!pathToFollow[currentIndex].IsNeighborAndActive(pathToFollow[nextIndex])) yield break;

            // Move to next index of the path
            MoveTo(nextIndex);
        }

        private void LookAtNode(NavNode targetNode)
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
    }
}