using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monument.World
{
    [System.Serializable]
    public class Walkable : MonoBehaviour
    {
        [SerializeField] private float offset = .5f;
        [SerializeField] private List<Neighbor> neighbors = new List<Neighbor>();

        public List<Neighbor> Neighbors => neighbors;

        public Vector3 WalkPoint => transform.position + Vector3.up * offset;

        public RotativePlatform RotativePlatform { get; set; } = null;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(WalkPoint, 0.1f);

            if (neighbors == null) return;

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i] == null || neighbors[i].Walkable == null) continue;

                if (neighbors[i].isActive)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(WalkPoint, neighbors[i].Walkable.WalkPoint);
                }
            }
        }

        public void AddAdjacentNeighbors()
        {
            RaycastHit hit;
            // Check 4 directions
            Vector3[] directions = new Vector3[] {
                transform.TransformDirection(Vector3.forward),
                transform.TransformDirection(Vector3.back),
                transform.TransformDirection(Vector3.left),
                transform.TransformDirection(Vector3.right),
            };

            for (int i = 0; i < directions.Length; i++)
            {
                if (Physics.Raycast(transform.position, directions[i], out hit))
                {
                    if (hit.distance <= 1)
                    {
                        Walkable target = hit.transform.gameObject.GetComponent<Walkable>();
                        if (target != null)
                        {
                            if (!IsNeighbor(target))
                            {
                                neighbors.Add(new Neighbor(target, true));
                            }
                        }
                    }

                }
            }// for            
        }

        public void SetNeighborActive(Walkable neighbor, bool active)
        {
            if (!IsNeighbor(neighbor)) return;

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i].Walkable == neighbor)
                {
                    neighbors[i].isActive = active;
                    break;
                }
            }

        }

        private bool IsNeighbor(Walkable walkable)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i].Walkable == walkable) return true;
            }

            return false;
        }

        public bool IsNeighborAndActive(Walkable walkable)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i].Walkable == walkable)
                {
                    return neighbors[i].isActive;
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public class Neighbor
    {
        public Walkable Walkable;
        public bool isActive = true;

        public Neighbor(Walkable walkable, bool isActive)
        {
            Walkable = walkable;
            this.isActive = isActive;
        }
    }

}