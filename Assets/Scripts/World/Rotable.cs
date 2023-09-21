using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotable : MonoBehaviour
{
    public enum RotateAxis { X, Y, Z }

    public RotateAxis SpinAxis = RotateAxis.X;

    public bool AllowsRotation { get; set; } = true;
}
