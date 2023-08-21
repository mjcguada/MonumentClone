using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Monument.World
{
    [RequireComponent(typeof(RotationSnapper))]
    public class RotatorHandle : RotatorInput
    {
        [SerializeField] private RotativePlatform platformToRotate;

        protected override void Start()
        {
#if UNITY_EDITOR
            if (platformToRotate == null)
            {
                Debug.LogError($"{gameObject.name} handle error. Platform to rotate reference is missing");
                gameObject.SetActive(false);
                return;
            }
#endif

            base.Start();

            // If the platform is linked to a handle, that's the only way to rotate it
            platformToRotate.inputEnabled = false;
        }

        public override void OnDrag(PointerEventData inputData)
        {
            base.OnDrag(inputData);

            Vector2 delta = inputData.position - pivotPosition;
            float rotationAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            platformToRotate.Rotate(rotationAngle);
        }

        public override void OnEndDrag(PointerEventData inputData)
        {
            base.OnEndDrag(inputData);

            platformToRotate.OnEndDrag(inputData);
        }
    }
}