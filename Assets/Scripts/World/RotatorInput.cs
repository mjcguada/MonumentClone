using UnityEngine;
using UnityEngine.EventSystems;

/**
 * This script defines a main class that implements abasic 
 * rotating input interaction and uses a RotationSnapper to 
 * always snap to a rotation angle that is a multiple of 90 degrees.
 */

namespace Monument.World
{
    [RequireComponent(typeof(RotationSnapper))]
    public class RotatorInput : Rotable, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Rotator Input")]
        public bool CanBeDisabled = true;

        protected Vector2 pivotPosition = default;

        protected float previousAngle = 0;

        protected RotationSnapper snapper;

        protected void Awake()
        {
            snapper = GetComponent<RotationSnapper>();
        }        

        public virtual void OnBeginDrag(PointerEventData inputData)
        {
            pivotPosition = Camera.main.WorldToScreenPoint(transform.position);

            snapper.StopSnap();

            Vector2 delta = inputData.position - pivotPosition;
            previousAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        }

        public virtual void OnDrag(PointerEventData inputData)
        {
            Vector2 delta = inputData.position - pivotPosition;
            float rotationAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            Rotate(rotationAngle);
        }

        public void Rotate(float rotationAngle)
        {
            if (SpinAxis == RotateAxis.X)
            {
                transform.Rotate(previousAngle - rotationAngle, 0, 0);
            }
            else if (SpinAxis == RotateAxis.Y)
            {
                transform.Rotate(0, previousAngle - rotationAngle, 0);
            }
            else
            {
                transform.Rotate(0, 0, rotationAngle - previousAngle);
            }
            previousAngle = rotationAngle;
        }

        public virtual void OnEndDrag(PointerEventData inputData)
        {
            float currentAngleRotation = transform.rotation.eulerAngles[(int)SpinAxis];
            float snappedAngleRotation = Mathf.Round(currentAngleRotation / 90.0f) * 90.0f;

            Vector3 eulerRotation = transform.rotation.eulerAngles;
            eulerRotation[(int)SpinAxis] = snappedAngleRotation;

            Quaternion snappedRotation = Quaternion.Euler(eulerRotation);

            snapper.StartSnap(snappedRotation, 0.25f);
        }

        public virtual void EnableRotation(bool enabled)
        {
            if (!CanBeDisabled) return;

            AllowsRotation = enabled;
        }
    }
}