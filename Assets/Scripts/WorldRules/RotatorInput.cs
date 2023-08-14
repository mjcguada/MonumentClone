using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Monument.World
{
    [RequireComponent(typeof(RotationSnapper))]
    public class RotatorInput : Rotable, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        protected Vector2 pivotPosition = default;

        protected float previousAngle = 0;

        protected RotationSnapper snapper;

        protected void Awake()
        {
            snapper = GetComponent<RotationSnapper>();
        }

        protected virtual void Start()
        {
            pivotPosition = Camera.main.WorldToScreenPoint(transform.position);
        }

        public virtual void OnBeginDrag(PointerEventData inputData)
        {
            snapper.StopSnap();

            Vector2 delta = inputData.position - pivotPosition;
            previousAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        }

        public virtual void OnDrag(PointerEventData inputData)
        {
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

        public virtual void OnEndDrag(PointerEventData inputData)
        {
            float currentAngleRotation = transform.rotation.eulerAngles[(int)spinAxis];
            float snappedAngleRotation = Mathf.Round(currentAngleRotation / 90.0f) * 90.0f;

            Vector3 eulerRotation = transform.rotation.eulerAngles;
            eulerRotation[(int)spinAxis] = snappedAngleRotation;            

            Quaternion snappedRotation = Quaternion.Euler(eulerRotation);

            snapper.StartSnap(snappedRotation, 0.25f);
        }
    }
}