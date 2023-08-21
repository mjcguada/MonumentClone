using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * This script defines a handle that controls
 * the rotation value of a RotativePlatform externally,
 * maintaining spin axes independent
 */

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
        }

        public override void OnBeginDrag(PointerEventData inputData)
        {
            base.OnBeginDrag(inputData);
            platformToRotate.PreviousAngle = this.previousAngle; // Important to start rotation with the same value
        }

        public override void OnDrag(PointerEventData inputData)
        {
            base.OnDrag(inputData);

            Vector2 delta = inputData.position - pivotPosition;
            float rotationAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            
            // We use Rotate function to maintain handle and platform spin axes independent
            platformToRotate.Rotate(rotationAngle);
        }

        public override void OnEndDrag(PointerEventData inputData)
        {
            base.OnEndDrag(inputData);
            platformToRotate.OnEndDrag(inputData);
        }
    }
}