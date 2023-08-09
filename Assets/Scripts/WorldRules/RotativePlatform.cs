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

        private float savedRotation = 0;

        private void Start()
        {
            SetupWalkableChildren();

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

            //Debug.Log("Current conf: " + currentConfiguration);
            
            if (currentConfiguration >= configurations.Length || configurations[currentConfiguration] == null) return;

            for (int i = 0; i < configurations[currentConfiguration].Linkers.Length; i++)
            {
                configurations[currentConfiguration].Linkers[i].ApplyConfiguration();
            }
        }


        private Vector2 mouseOriginPosition = default;

        void OnMouseDown()
        {
            if(!AllowsRotation) return;

            StopAllCoroutines();
            mouseOriginPosition = Input.mousePosition;
        }

        // Find closest rotation
        private void OnMouseUp()
        {
            if(!AllowsRotation) return;

            savedRotation = GetRotationAngle();
            if (savedRotation > 180) savedRotation = savedRotation - 360;

            Debug.Log($"Saved rotation: {savedRotation}");

            // Round angle to multiple of 90
            //int value = 360 - (int)GetRotationAngle();
            //int value = (int)GetRotationAngle();
            ////savedRotation = value;
            //int factor = 90;
            //int nearestMultiple = (int)Math.Round((value / (double)factor), MidpointRounding.AwayFromZero) * factor;
            
            //Debug.Log("Nearest: " + nearestMultiple);

            //StartCoroutine(RotateCoroutine(nearestMultiple, 0.25f));
        }

        private float GetRotationAngle() 
        {            
            return transform.rotation.eulerAngles[(int)rotationAxis];
        }

        private void OnMouseDrag()
        {
            if(!AllowsRotation) return;

            Vector2 delta = (Vector2)Input.mousePosition - mouseOriginPosition;
            float rotationAngle = Mathf.Atan2(delta.y, delta.x) * 180f / Mathf.PI;

            Debug.Log($"Rotation: {rotationAngle}");
            rotationAngle = 360 - (rotationAngle - savedRotation);

            //rotationAngle += savedRotation;

            if (rotationAxis == RotateAxis.X)
            {
                transform.rotation = Quaternion.Euler(rotationAngle, 0, 0);
            }
            else if (rotationAxis == RotateAxis.Y)
            {
                transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
            }
            //else
            //{
            //    transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
            //}
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

            //savedRotation = GetRotationAngle() - 360;

            //Debug.Log("Saved rotation: " + savedRotation);

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
