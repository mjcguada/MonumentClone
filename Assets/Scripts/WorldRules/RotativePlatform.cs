using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monument.World
{
    public class RotativePlatform : MonoBehaviour
    {
        public enum RotateAxis { X, Y, Z }

        [SerializeField] private RotateAxis rotateAxis = RotateAxis.X;

        [SerializeField] private PlatformConfiguration[] configurations;

        [HideInInspector] public bool CanRotate = true;

        private Walkable[] walkableChild;

        private int currentRotation = 0;

        private void Start()
        {
            currentRotation = (int)transform.rotation.eulerAngles.x;
            currentRotation = (currentRotation / 90) * 90; //round to multiple of 90

            // Start my Walkable list
            walkableChild = GetComponentsInChildren<Walkable>();

            for (int i = 0; i < walkableChild.Length; i++)
            {
                walkableChild[i].RotativePlatform = this;
            }

            ApplyConfiguration(currentRotation);
        }


        public void Rotate()
        {
            if (!CanRotate) return;

            StopAllCoroutines();

            currentRotation += 90;

            if (currentRotation >= 360) currentRotation = 0;

            ApplyConfiguration(currentRotation);
            Quaternion targetRotation = Quaternion.identity;

            if (rotateAxis == RotateAxis.X)
            {
                targetRotation = Quaternion.Euler(currentRotation, 0, 0);
            }
            else if (rotateAxis == RotateAxis.Y)
            {
                targetRotation = Quaternion.Euler(0, currentRotation, 0);
            }
            else
            {
                targetRotation = Quaternion.Euler(0, 0, currentRotation);
            }

            StartCoroutine(RotateCoroutine(targetRotation, 0.5f));
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

            //Apply set of linkers given current rotation
            int currentConfiguration = targetRotation / 90;

            if (currentConfiguration >= configurations.Length) return;

            for (int i = 0; i < configurations[currentConfiguration].Linkers.Length; i++)
            {
                configurations[currentConfiguration].Linkers[i].ApplyConfiguration();
            }
        }

        IEnumerator RotateCoroutine(Quaternion targetRotation, float timeToComplete)
        {
            Quaternion startingRotation = transform.rotation;
            float elapsedTime = 0;

            while (elapsedTime < timeToComplete)
            {
                transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / timeToComplete);
                yield return null;

                elapsedTime += Time.deltaTime;
            }
            transform.rotation = targetRotation;
        }
    }
}
