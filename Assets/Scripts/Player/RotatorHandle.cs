using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monument.World;

namespace Monument.Player
{
    public class RotatorHandle : MonoBehaviour
    {
        [SerializeField] private RotativePlatform rotativePlatform;

        private bool isActive = true; //is false when the player steps into the platform

#if UNITY_EDITOR
        private void Start()
        {
            if (rotativePlatform == null) 
            {
                Debug.LogError("Rotative platform missing: " + gameObject.name);
                Destroy(this);
            }
        }
#endif

        private void OnMouseDown()
        {
            if(isActive) rotativePlatform.Rotate();
        }
    }
}
