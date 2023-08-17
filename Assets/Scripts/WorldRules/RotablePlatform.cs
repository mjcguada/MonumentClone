using System.Collections;
using UnityEngine;

namespace Monument.World
{
    [RequireComponent(typeof(RotationSnapper))]
    public class RotablePlatform : Rotable, ISnappable
    {
        [SerializeField] private PlatformConfiguration[] configurations;

        // TODO: change name
        private NavNode[] walkableChild; 

        private RotationSnapper snapper;

        private void Awake() 
        {
            snapper = GetComponent<RotationSnapper>();
        }

        private void Start()
        {
            snapper.OnSnapFinished = ApplyConfiguration;

            SetupWalkableChildren();

            ApplyConfiguration();
        }

        private void SetupWalkableChildren()
        {
            // Start my Walkable list
            walkableChild = GetComponentsInChildren<NavNode>();

            for (int i = 0; i < walkableChild.Length; i++)
            {
                //walkableChild[i].RotativePlatform = this;
            }
        }

        public void ApplyConfiguration()
        {
            //We always activate every child, then we apply the current configuration
            foreach (var walkableChild in walkableChild)
            {
                foreach (var neighbor in walkableChild.Neighbors)
                {
                    neighbor.isActive = true;
                }
            }

            // Establish desired configuration based on current rotation
            float currentAngleRotation = transform.rotation.eulerAngles[(int)spinAxis];
            float snappedAngleRotation = Mathf.Round(currentAngleRotation / 90.0f) * 90.0f;

            int currentConfiguration = (int)snappedAngleRotation / 90;

            if (currentConfiguration >= configurations.Length || configurations[currentConfiguration] == null) return;

            //Apply set of linkers given current rotation
            for (int i = 0; i < configurations[currentConfiguration].Linkers.Length; i++)
            {
                configurations[currentConfiguration].Linkers[i].ApplyConfiguration();
            }
        }

        public void StartSnap(Quaternion targetRotation, float timeToComplete)
        {
            snapper.StartSnap(targetRotation, timeToComplete);
        }

        public void StopSnap()
        {
            snapper.StopSnap();
        }
    }
}
