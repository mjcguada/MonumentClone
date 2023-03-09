using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Walkable : MonoBehaviour
{
    [SerializeField] private float offset = .5f;
    [SerializeField] private WalkableNeighbor[] neighbors;

    public Vector3 WalkPoint => transform.position + transform.up * offset;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(WalkPoint, 0.1f);

        if (neighbors == null) return;

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] == null || neighbors[i].target == null) continue;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(WalkPoint, neighbors[i].target.WalkPoint);
        }
    }
}

[System.Serializable]
public class WalkableNeighbor
{
    public Walkable target;
    public bool active = true;
}