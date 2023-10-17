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

        // Scale animation parameters
        private Vector3 originalScale = default;
        private const float scaleAnimationTime = 0.3f;
        private Coroutine scaleCoroutine = null;

        private void Start()
        {
#if UNITY_EDITOR
            if (platformToRotate == null)
            {
                Debug.LogError($"{gameObject.name} handle error. Platform to rotate reference is missing");
                gameObject.SetActive(false);
                return;
            }
#endif
            originalScale = transform.localScale;
        }

        public override void OnBeginDrag(PointerEventData inputData)
        {
            if (!AllowsRotation) return;

            platformToRotate.OnBeginDrag(inputData);

            base.OnBeginDrag(inputData);
            platformToRotate.PreviousAngle = this.previousAngle; // Important to start rotation with the same value
        }

        public override void OnDrag(PointerEventData inputData)
        {
            if (!AllowsRotation) return;

            base.OnDrag(inputData);

            Vector2 delta = inputData.position - pivotPosition;
            float rotationAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            // We use Rotate function to maintain handle and platform spin axes independent
            platformToRotate.Rotate(rotationAngle);
        }

        public override void OnEndDrag(PointerEventData inputData)
        {
            if (!AllowsRotation) return;

            base.OnEndDrag(inputData);
            platformToRotate.OnEndDrag(inputData);
        }

        public void EnableRotation(bool enabled)
        {
            AllowsRotation = enabled;

            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            
            // Lerp Scale Coroutine (targetScale depends on enabled value)
            StartCoroutine(LerpScaleCoroutine(enabled ? originalScale : originalScale * 0.5f, scaleAnimationTime));
        }

        private IEnumerator LerpScaleCoroutine(Vector3 targetScale, float timeToComplete)
        {
            Vector3 startingScale = transform.localScale;
            float elapsedTime = 0;

            while (elapsedTime < timeToComplete)
            {
                elapsedTime += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / timeToComplete);
                yield return null;
            }
        }
    }
}