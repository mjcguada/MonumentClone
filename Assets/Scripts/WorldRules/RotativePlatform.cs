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

        private NavNode[] childrenNodes = null;

        public float PreviousAngle { set => previousAngle = value; }

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

            if (currentConfiguration >= configurations.Length || configurations[currentConfiguration] == null) return;

            // Apply linkers given the current rotation
            Linker[] configurationLinkers = configurations[currentConfiguration].Linkers;

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