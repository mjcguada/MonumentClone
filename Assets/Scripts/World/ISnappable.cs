using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISnappable 
{
    public void StartSnap(Quaternion targetRotation, float timeToComplete);

    public void StopSnap();
}
