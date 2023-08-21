using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Monument.World
{
    [RequireComponent(typeof(RotationSnapper))]
    public class RotativePlatform : RotatorInput
    {
        [SerializeField]
        private PlatformConfiguration[] configurations = new PlatformConfiguration[4];

        private NavNode[] childrenNodes;

        public bool inputEnabled = true;

        protected override void Start()
        {
            base.Start();

            snapper.OnSnapFinished = ApplyConfiguration;

            SetupWalkableChildren();

            ApplyConfiguration();
        }

        private void SetupWalkableChildren()
        {
            // Start my Walkable list
            childrenNodes = GetComponentsInChildren<NavNode>();

            for (int i = 0; i < childrenNodes.Length; i++)
            {
                childrenNodes[i].RotativePlatform = this;
            }
        }

        public void ApplyConfiguration()
        {
            //We always activate every child, then we apply the current configuration
            foreach (var walkableChild in childrenNodes)
            {
                foreach (var neighbor in walkableChild.Neighbors)
                {
                    neighbor.isActive = true;
                }
            }

            // Establish desired configuration based on current rotation
            float currentAngleRotation = transform.rotation.eulerAngles[(int)SpinAxis];
            float snappedAngleRotation = Mathf.Round(currentAngleRotation / 90.0f) * 90.0f;

            int currentConfiguration = (int)snappedAngleRotation / 90;

            if (currentConfiguration >= configurations.Length || configurations[currentConfiguration] == null) return;

            //Apply set of linkers given current rotation
            for (int i = 0; i < configurations[currentConfiguration].Linkers.Length; i++)
            {
                configurations[currentConfiguration].Linkers[i].ApplyConfiguration();
            }
        }

        public override void OnBeginDrag(PointerEventData inputData)
        {
            if (!AllowsRotation || !inputEnabled) return;

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
