using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * This script defines an interactive platform, that is
 * capable of rotating by adjusting to multiples of 90 degrees
 */

namespace Monument.World
{
    [RequireComponent(typeof(RotationSnapper))]
    public class RotativePlatform : RotatorInput
    {
        [SerializeField]
        private PlatformConfiguration[] configurations = new PlatformConfiguration[4];

        [SerializeField]
        public RotatorHandle RotatorHandle;

        public float PreviousAngle { set => previousAngle = value; }

        private int previousConfiguration = -1;
        private NavNode[] childrenNodes = null;

        protected override void Start()
        {
            base.Start();

            snapper.OnSnapFinished = ApplyConfiguration;

            AssignPlatformToChildrenNodes();

            ApplyConfiguration();
        }

        private void AssignPlatformToChildrenNodes()
        {
            // Find Node children inside the gameObject
            childrenNodes = GetComponentsInChildren<NavNode>();

            for (int i = 0; i < childrenNodes.Length; i++)
            {
                childrenNodes[i].RotativePlatform = this;
            }
        }

        public void ApplyConfiguration()
        {
            // Establish desired configuration based on current rotation
            float currentAngleRotation = transform.rotation.eulerAngles[(int)SpinAxis];
            float snappedAngleRotation = Mathf.Round(currentAngleRotation / 90.0f) * 90.0f;

            int currentConfiguration = (int)snappedAngleRotation / 90;

            if (currentConfiguration == previousConfiguration) return;

            if (configurations[currentConfiguration] == null)
            {
                // New configuration
                previousConfiguration = currentConfiguration;
                return;
            }

            if (previousConfiguration >= 0)
            {
                // Undo previous configuration
                Linker[] previousConfigurationLinkers = configurations[previousConfiguration].Linkers;

                for (int i = 0; i < previousConfigurationLinkers.Length; i++)
                {
                    previousConfigurationLinkers[i].ApplyConfiguration(!previousConfigurationLinkers[i].areLinked);
                }
            }

            // New configuration
            previousConfiguration = currentConfiguration;

            // Apply linkers given the current rotation
            Linker[] configurationLinkers = configurations[currentConfiguration].Linkers;
            if (configurationLinkers == null) return;

            for (int i = 0; i < configurationLinkers.Length; i++)
            {
                configurationLinkers[i].ApplyConfiguration(configurationLinkers[i].areLinked);
            }
        }

        public override void OnBeginDrag(PointerEventData inputData)
        {
            if (!AllowsRotation) return;

            base.OnBeginDrag(inputData);
        }

        public override void OnDrag(PointerEventData inputData)
        {
            if (!AllowsRotation) return;

            base.OnDrag(inputData);
        }

        // Snap to a 90 degree configuration
        public override void OnEndDrag(PointerEventData inputData)
        {
            if (!AllowsRotation) return;

            base.OnEndDrag(inputData);
        }
    }
}