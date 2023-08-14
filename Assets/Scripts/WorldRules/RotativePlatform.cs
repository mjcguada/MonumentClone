using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Monument.World
{
    [RequireComponent(typeof(RotationSnapper))]
    public class RotativePlatform : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public enum RotateAxis { X, Y, Z }

        [SerializeField] private RotateAxis spinAxis = RotateAxis.X;

        [SerializeField] private PlatformConfiguration[] configurations;

        public bool AllowsRotation { get; set; } = true;

        private Walkable[] walkableChild;

        private Vector2 pivotPosition = default;

        private float previousAngle = 0;

        private RotationSnapper snapper;

        private void Awake() 
        {
            snapper = GetComponent<RotationSnapper>();
        }

        private void Start()
        {
            SetupWalkableChildren();

            pivotPosition = Camera.main.WorldToScreenPoint(transform.position);

            ApplyConfiguration();
        }

        private void SetupWalkableChildren()
        {
            // Start my Walkable list
            walkableChild = GetComponentsInChildren<Walkable>();

            for (int i = 0; i < walkableChild.Length; i++)
            {
                walkableChild[i].RotativePlatform = this;
            }
        }

        private void ApplyConfiguration()
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

        public void OnBeginDrag(PointerEventData inputData)
        {
            if (!AllowsRotation) return;

            snapper.StopSnapCoroutine();

            Vector2 delta = inputData.position - pivotPosition;
            previousAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        }

        public void OnDrag(PointerEventData inputData)
        {
            if (!AllowsRotation) return;

            Vector2 delta = inputData.position - pivotPosition;
            float rotationAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            if (spinAxis == RotateAxis.X)
            {
                transform.Rotate(previousAngle - rotationAngle, 0, 0);
            }
            else if (spinAxis == RotateAxis.Y)
            {
                transform.Rotate(0, previousAngle - rotationAngle, 0);
            }
            else
            {
                transform.Rotate(0, 0, rotationAngle - previousAngle);
            }

            previousAngle = rotationAngle;
        }

        // Snap to a 90 degree configuration
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!AllowsRotation) return;

            float currentAngleRotation = transform.rotation.eulerAngles[(int)spinAxis];
            float snappedAngleRotation = Mathf.Round(currentAngleRotation / 90.0f) * 90.0f;

            Vector3 eulerRotation = transform.rotation.eulerAngles;
            eulerRotation[(int)spinAxis] = snappedAngleRotation;

            Quaternion snappedRotation = Quaternion.Euler(eulerRotation);

            snapper.InitSnapCoroutine(snappedRotation, 0.25f, ApplyConfiguration);
        }
    }
}
