using System;
using System.Collections;
using UnityEngine;

namespace Monument.World
{
    public class RotativePlatform : MonoBehaviour
    {
        public enum RotateAxis { X, Y, Z }

        [SerializeField] private RotateAxis rotationAxis = RotateAxis.X;

        [SerializeField] private PlatformConfiguration[] configurations;

        public bool AllowsRotation { get; set; } = true;

        private Walkable[] walkableChild;

        private Vector2 pivotPosition = default;

        private Coroutine snapCoroutine = null;

        private void Start()
        {
            SetupWalkableChildren();

            pivotPosition = Camera.main.WorldToScreenPoint(transform.position);

            // TODO: round default rotation and apply conf

            //ApplyConfiguration(currentRotation);
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

        private void ApplyConfiguration(int targetRotation)
        {
            //We always activate every child, then we apply the current configuration
            foreach (var walkableChild in walkableChild)
            {
                foreach (var neighbor in walkableChild.Neighbors)
                {
                    neighbor.isActive = true;
                }
            }

            if (targetRotation >= 360) targetRotation = targetRotation - 360;

            //Apply set of linkers given current rotation
            int currentConfiguration = targetRotation / 90;

            if (currentConfiguration >= configurations.Length || configurations[currentConfiguration] == null) return;

            for (int i = 0; i < configurations[currentConfiguration].Linkers.Length; i++)
            {
                configurations[currentConfiguration].Linkers[i].ApplyConfiguration();
            }
        }

        void OnMouseDown()
        {
            if (!AllowsRotation) return;

            if (snapCoroutine != null) StopCoroutine(snapCoroutine);

            Vector2 delta = (Vector2)Input.mousePosition - pivotPosition;
            previousAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        }

        // Find closest rotation
        private void OnMouseUp()
        {
            if (!AllowsRotation) return;

            // Round rotation angle to multiple of 90
            int value = (int)GetRotationAngle();
            int factor = 90;
            int nearestMultiple = (int)Math.Round((value / (double)factor), MidpointRounding.AwayFromZero) * factor;

            snapCoroutine = StartCoroutine(RotateCoroutine(nearestMultiple, 0.25f));
        }

        private float GetRotationAngle()
        {
            return transform.rotation.eulerAngles[(int)rotationAxis];
        }

        private float previousAngle = 0;

        private void OnMouseDrag()
        {
            if (!AllowsRotation) return;

            Vector2 delta = (Vector2)Input.mousePosition - pivotPosition;
            float rotationAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            if (rotationAxis == RotateAxis.X)
            {
                transform.Rotate(previousAngle - rotationAngle, 0, 0);
            }
            else if (rotationAxis == RotateAxis.Y)
            {
                transform.Rotate(0, previousAngle - rotationAngle, 0);
            }
            else
            {
                transform.Rotate(0, 0, rotationAngle - previousAngle);
            }

            previousAngle = rotationAngle;
        }

        IEnumerator RotateCoroutine(int targetAngle, float timeToComplete)
        {
            Quaternion startingRotation = transform.rotation;
            Quaternion targetRotation = GenerateQuaternion(targetAngle);

            float elapsedTime = 0;

            while (elapsedTime < timeToComplete)
            {
                transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / timeToComplete);
                yield return null;

                elapsedTime += Time.deltaTime;
            }
            transform.rotation = targetRotation;

            ApplyConfiguration(targetAngle);
        }

        private Quaternion GenerateQuaternion(int targetAngle)
        {
            if (rotationAxis == RotateAxis.X)
            {
                return Quaternion.Euler(targetAngle, 0, 0);
            }
            else if (rotationAxis == RotateAxis.Y)
            {
                return Quaternion.Euler(0, targetAngle, 0);
            }
            else
            {
                return Quaternion.Euler(0, 0, targetAngle);
            }
        }
    }
}
