using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotable : MonoBehaviour
{
    public enum RotateAxis { X, Y, Z }

    [SerializeField] protected RotateAxis spinAxis = RotateAxis.X;

    public bool AllowsRotation { get; set; } = true;
}
