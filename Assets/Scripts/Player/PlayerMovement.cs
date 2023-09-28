using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monument.World;

namespace Monument.Player
{
    public class PlayerMovement : Walker
    {
        [SerializeField] private PlayerAnimator animator;
        [SerializeField] private PlayerCursor cursor;

        // Called on click action to find a new path and wait for the older one to finish
        private Coroutine findPathCoroutine = null;

        private MonumentInput inputActions;

        // The last rotative platform used by the player
        private RotativePlatform lastRotativePlatform;

        // Reachable nodes (possible paths)
        List<NavNode> reachableNodes = new List<NavNode>();

        private void Awake()
        {
            OnMovementInterrupted = OnPlayerStopped;
        }

        void Start()
        {
            inputActions = new MonumentInput();
            inputActions.Player.Click.performed += ctx => OnClick();
            inputActions.Enable();

            FindReachableNodes(FindNodeUnderPlayer());
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

            NavNode originNode = FindNodeUnderPlayer();

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

            List<NavNode> path = NavNodePathFinder.FindPathFrom(originNode, target);
            if (path != null)
            {
                FollowPath(path);
            }
            else
            {
                Debug.Log("Path not found.");
            }
        }

        private void FollowPath(List<NavNode> path)
        {
            animator.Walking(true);

            pathToFollow = path;
            MoveTo(currentIndex: 1); // index 0 is the origin node
        }

        protected override void MoveTo(int currentIndex)
        {
            // if the given index is smaller than the length of the list
            // we continue moving
            if (currentIndex < pathToFollow.Count)
            {
                isMoving = true;

                // Show reachable nodes on the Editor
                FindReachableNodes(pathToFollow[currentIndex]);

                // Enable again rotative platform that we leave
                if (lastRotativePlatform != null)
                {
                    // Handle rotation
                    if (lastRotativePlatform.RotatorHandle != null) lastRotativePlatform.RotatorHandle.EnableRotation(true);

                    // Platform rotation
                    lastRotativePlatform.AllowsRotation = true;

                    transform.SetParent(null);
                }

                // Assign current rotative platform and disable rotation
                if (pathToFollow[currentIndex].RotativePlatform != null)
                {
                    lastRotativePlatform = pathToFollow[currentIndex].RotativePlatform;

                    // Disable Handle rotation
                    if (lastRotativePlatform.RotatorHandle != null) lastRotativePlatform.RotatorHandle.EnableRotation(false);

                    // Disable Platform rotation (while the player is moving)
                    lastRotativePlatform.AllowsRotation = false;

                    // Make player child of rotative platform to rotate with it
                    transform.SetParent(lastRotativePlatform.transform, true);
                }

                // Make the player child of the platform to be affected by its rotation

                // Look at next node
                LookAtNode(pathToFollow[currentIndex]);

                // Move to next node
                StartCoroutine(MoveToNodeCoroutine(currentIndex));
            }
            //else
            //{
            //    OnPlayerStopped();
            //}
        }

        private void OnPlayerStopped()
        {
            isMoving = false;
            animator.Walking(false);

            // Enable rotative platform
            if (lastRotativePlatform != null)
            {
                // Platform rotation
                lastRotativePlatform.AllowsRotation = true;
            }
        }

        private void FindReachableNodes(NavNode originNode)
        {
#if UNITY_EDITOR
            // Clear previous list of nodes
            for (int i = 0; i < reachableNodes.Count; i++)
            {
                reachableNodes[i].IsReachable = false;
            }
            reachableNodes.Clear();

            // Find new nodes
            reachableNodes = NavNodePathFinder.FindReachableNodesFrom(originNode);
            for (int i = 0; i < reachableNodes.Count; i++)
            {
                reachableNodes[i].IsReachable = true;
            }
        }
#endif
    }
}