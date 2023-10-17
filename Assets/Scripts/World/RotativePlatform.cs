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
    public class RotativePlatform : RotatorInput, IReactor
    {
        [SerializeField]
        private PlatformConfiguration[] configurations = new PlatformConfiguration[4];

        [SerializeField]
        public RotatorHandle RotatorHandle;

        public float PreviousAngle { set => previousAngle = value; }

        private int previousConfiguration = -1;
        private NavNode[] childrenNodes = null;

        private void Start()
        {
            snapper.OnSnapFinished += delegate { SetLinkersActive(true); };
            snapper.OnSnapFinished += ApplyConfiguration;

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

        // Disables/Enables the last saved linkers configuration while the user is interacting with the platform
        private void SetLinkersActive(bool active)
        {
            ApplyLinkersConfiguration(previousConfiguration, !active);
        }

        private void ApplyLinkersConfiguration(int linkersIndex, bool undo)
        {
            if (linkersIndex < 0 || linkersIndex >= configurations.Length) return;

            Linker[] configurationLinkers = configurations[linkersIndex].Linkers;

            if (configurationLinkers == null) return;

            for (int i = 0; i < configurationLinkers.Length; i++)
            {
                if (undo) configurationLinkers[i].ApplyConfiguration(!configurationLinkers[i].areLinked);
                else configurationLinkers[i].ApplyConfiguration(configurationLinkers[i].areLinked);
            }
        }

        public void ApplyConfiguration()
        {
            // Establish desired configuration based on current rotation
            float currentAngleRotation = transform.rotation.eulerAngles[(int)SpinAxis];
            float snappedAngleRotation = Mathf.Round(currentAngleRotation / 90.0f) * 90.0f;

            int currentConfiguration = (int)snappedAngleRotation / 90;

            // Undo previous configuration
            ApplyLinkersConfiguration(previousConfiguration, undo: true);

            // Apply linkers given the current rotation            
            ApplyLinkersConfiguration(currentConfiguration, undo: false);

            // Update nodes' Walkpoint to current rotation
            if (childrenNodes == null || childrenNodes.Length == 0) AssignPlatformToChildrenNodes();
            for (int i = 0; i < childrenNodes.Length; i++)
            {
                childrenNodes[i].ApplyConfiguration(currentConfiguration);
            }

            // New configuration
            previousConfiguration = currentConfiguration;            
        }

        public override void OnBeginDrag(PointerEventData inputData)
        {
            if (!AllowsRotation) return;

            SetLinkersActive(false);

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

        public void React(Reaction reaction)
        {
            if (reactionCoroutine != null) StopCoroutine(reactionCoroutine);

            switch (reaction.Type)
            {
                case Reaction.ReactionType.Rotation:
                    reactionCoroutine = StartCoroutine(ReactionRotation(reaction));
                    break;
            }
        }

        private Coroutine reactionCoroutine = null;

        // Progressive rotation the platform does when a Walker steps into a Pressure Plate
        private IEnumerator ReactionRotation(Reaction reaction)
        {
            float elapsedTime = 0;

            while (elapsedTime < reaction.TimeToComplete)
            {
                elapsedTime += Time.deltaTime;
                float currentAngle = Mathf.Lerp(0, reaction.Units, elapsedTime / reaction.TimeToComplete); // We calculate the appropriate rotation angle in relation to the elapsed time
                Rotate(currentAngle);
                yield return null;
            }

            ApplyConfiguration(); //Apply resulting configuration
        }
    }
}