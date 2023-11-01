using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monument.World;
using UnityEngine.InputSystem;

namespace Monument.Player
{
    public class PlayerMovement : Walker, IInteractor
    {
        [SerializeField] private PlayerAnimator animator;
        [SerializeField] private LayerMask navNodeLayerMask;

        // Called on click action to find a new path and wait for the older one to finish
        private Coroutine findPathCoroutine = null;

        private List<NavNode> pathToFollow = new List<NavNode>();
        private int currentIndex = 0;

        private MonumentInput inputActions;

        // Reachable nodes (possible paths)
        List<NavNode> reachableNodes = new List<NavNode>();        

        private void OnEnable()
        {
            if (inputActions == null) 
            {
                inputActions = new MonumentInput();
                inputActions.Player.Click.performed += OnClick;
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions?.Disable();
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            Vector2 inputVector = context.ReadValue<Vector2>();

            Ray ray = Camera.main.ScreenPointToRay(inputVector);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, navNodeLayerMask))
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
            
            // Wait until the player reaches the Node they were moving to
            while (isMoving)
            {
                yield return null;
            }

            currentNode = FindNodeUnderCharacter();

            if (currentNode == null)
            {
                Debug.LogError("Couldn't find origin node");
                yield break;
            }
            else if (currentNode == target)
            {
                Debug.Log("Target and origin nodes are the same one");
                yield break;
            }            

            List<NavNode> path = NavNodePathFinder.FindPathFrom(currentNode, target);
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
            currentIndex = 0;
            MoveToNextNode(); // index 0 is the origin node
        }

        private void MoveToNextNode()
        {
            currentIndex++;            

            // We stope if the player has reached the end of the path
            if (currentIndex >= pathToFollow.Count)
            {
                OnPlayerStopped();
                return;
            }

            // Before moving to the next Node, we have to check if it's still an active neighbor
            if (currentNode != null && !currentNode.Neighbors.Contains(pathToFollow[currentIndex]))
            {
                OnPlayerStopped();
                return;
            }

#if UNITY_EDITOR
            // Show reachable nodes on the Editor
            FindReachableNodes(pathToFollow[currentIndex]);
#endif
            // Disable previous platform and assign the current one
            ManageRotativePlatforms(pathToFollow[currentIndex].RotativePlatform);

            NavNode targetNode = pathToFollow[currentIndex];

            // Look at next node
            LookAtNode(targetNode);

            // Move to next node
            StartCoroutine(MoveToNodeCoroutine(currentNode, targetNode, OnMovementFinished: MoveToNextNode));
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

#if UNITY_EDITOR
        private void FindReachableNodes(NavNode originNode)
        {
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