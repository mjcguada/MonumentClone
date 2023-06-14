using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monument.World;

namespace Monument.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private MonumentInput inputActions;

        private RotativePlatform currentPlatform = null; //to be able to enable/disable platforms on runtime

        //The last rotative platform used by the player
        private RotativePlatform lastRotativePlatform;

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
                Walkable target = hit.transform.gameObject.GetComponent<Walkable>();
                if (target != null)
                {
                    List<Walkable> path = FindPath(null, target);
                }
            }
        }

        private List<Walkable> FindPath(Walkable origin, Walkable target)
        {
            List<Walkable> path = new List<Walkable>();

            if (origin == null)
            {
                Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
                Collider[] colliders = Physics.OverlapSphere(spherePosition, 0.2f);

                if (colliders != null && colliders.Length > 0) origin = colliders[0].gameObject.GetComponent<Walkable>();
            }

            if (origin != null)
            {
                //We create the same amount of possible paths as neighbors has the origin
                List<List<Walkable>> possiblePaths = new List<List<Walkable>>();

                for (int i = 0; i < origin.Neighbors.Count; i++)
                {
                    if (!origin.Neighbors[i].isActive) continue;

                    List<Walkable> neighborPath = new List<Walkable>();
                    neighborPath.Add(origin);
                    AddNeighbors(ref neighborPath, origin.Neighbors[i].Walkable, target);

                    possiblePaths.Add(neighborPath);
                }

                for (int i = 0; i < possiblePaths.Count; i++)
                {
                    if (possiblePaths[i].Contains(target))
                    {
                        StopAllCoroutines();
                        FollowPath(possiblePaths[i]);
                        break;
                    }
                }
            }

            return path;
        }

        private void AddNeighbors(ref List<Walkable> path, Walkable currentWalkable, Walkable target)
        {
            path.Add(currentWalkable);

            if (currentWalkable.Neighbors == null) return;

            for (int i = 0; i < currentWalkable.Neighbors.Count; i++)
            {
                Neighbor currentNeighbor = currentWalkable.Neighbors[i];

                if (!currentNeighbor.isActive) continue;

                if (!path.Contains(currentNeighbor.Walkable))
                {
                    if (currentNeighbor.Walkable == target)
                    {
                        path.Add(currentNeighbor.Walkable); //Goal found
                        break; //or return path
                    }
                    else
                    {
                        AddNeighbors(ref path, currentNeighbor.Walkable, target);
                    }
                }
            }

        }

        private void MoveTo(List<Walkable> path, int targetIndex, float timeToArrive)
        {
            if (path.Count > targetIndex)
            {
                if (lastRotativePlatform != null) lastRotativePlatform.CanRotate = true;

                if (path[targetIndex].RotativePlatform != null)
                {
                    lastRotativePlatform = path[targetIndex].RotativePlatform;
                    lastRotativePlatform.CanRotate = false;
                }

                StartCoroutine(MoveToPosition(path[targetIndex].WalkPoint, targetIndex, path, timeToArrive, MoveTo));
            }
        }

        IEnumerator MoveToPosition(Vector3 targetPosition, int currentIndex, List<Walkable> path, float timeToArrive,
            System.Action<List<Walkable>, int, float> callback)
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

        private void FollowPath(List<Walkable> path)
        {
            MoveTo(path, 1, 0.25f);
        }

    }
}
