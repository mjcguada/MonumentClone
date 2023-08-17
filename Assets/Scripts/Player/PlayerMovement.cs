using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monument.World;

namespace Monument.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private MonumentInput inputActions;

        //The last rotative platform used by the player
        private Rotable lastRotativePlatform;

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
                NavNode target = hit.transform.gameObject.GetComponent<NavNode>();
                if (target != null)
                {
                    Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
                    Collider[] colliders = Physics.OverlapSphere(spherePosition, 0.2f);

                    if (colliders != null && colliders.Length > 0)
                    {
                        NavNode origin = colliders[0].gameObject.GetComponent<NavNode>();
                        FindPath(origin, target);
                    }
                }
            }
        }

        private void FindPath(NavNode origin, NavNode target)
        {
            Queue<NavNode> queue = new Queue<NavNode>();
            queue.Enqueue(origin);

            Dictionary<NavNode, NavNode> parentMap = new Dictionary<NavNode, NavNode>();
            parentMap[origin] = null;

            bool pathFound = false;

            while (queue.Count > 0)
            {
                NavNode current = queue.Dequeue();

                if (current.Equals(target))
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
                StopAllCoroutines();
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


        private void MoveTo(List<NavNode> path, int targetIndex, float timeToArrive)
        {
            if (path.Count > targetIndex)
            {
                if (lastRotativePlatform != null) lastRotativePlatform.AllowsRotation = true;

                if (path[targetIndex].RotativePlatform != null)
                {
                    lastRotativePlatform = path[targetIndex].RotativePlatform;
                    lastRotativePlatform.AllowsRotation = false;
                }

                StartCoroutine(MoveToPosition(path[targetIndex].WalkPoint, targetIndex, path, timeToArrive, MoveTo));
            }
        }

        IEnumerator MoveToPosition(Vector3 targetPosition, int currentIndex, List<NavNode> path, float timeToArrive,
            System.Action<List<NavNode>, int, float> callback)
        {
            float elapsedTime = 0;
            Vector3 startingPos = transform.position;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / timeToArrive);
                yield return null;
            }

            if (currentIndex + 1 >= path.Count) yield break;

            //Before moving to the next Walkable, we have to check if it's still a neighbor (something could have changed)
            if (!path[currentIndex].IsNeighborAndActive(path[currentIndex + 1])) yield break;

            callback(path, currentIndex + 1, timeToArrive);
        }

        private void FollowPath(List<NavNode> path)
        {
            MoveTo(path, 1, 0.25f);
        }

    }
}
