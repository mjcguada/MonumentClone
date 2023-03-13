using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Monument.World
{
    public class RotativePlatform : MonoBehaviour
    {
        public enum RotateAxis { X, Y, Z }

        [SerializeField] private RotateAxis rotateAxis = RotateAxis.X;

        private List<Walkable> walkableChild = new List<Walkable>();

        private int currentRotation = 0;

        private void Start()
        {
            currentRotation = (int)transform.rotation.eulerAngles.x;
            currentRotation = ((int)currentRotation / 90) * 90; //round to multiple of 90

            Walkable[] walkables = GetComponentsInChildren<Walkable>();
            walkableChild = walkables.ToList();

            for (int i = 0; i < walkableChild.Count; i++)
            {
                walkableChild[i].rotativePlatform = this;
            }
        }


        public void Rotate()
        {
            StopAllCoroutines();

            if (rotateAxis == RotateAxis.X)
            {
                currentRotation += 90;

                if (currentRotation >= 360) currentRotation = 0;

                Quaternion targetRotation = Quaternion.Euler(currentRotation, 0, 0);

                StartCoroutine(RotateCoroutine(targetRotation, 0.5f));
            }
            else if (rotateAxis == RotateAxis.Y)
            {

            }
            else 
            {
            
            }
        }

        IEnumerator RotateCoroutine(Quaternion targetRotation, float timeToComplete)
        {
            Quaternion startingRotation = transform.rotation;
            float elapsedTime = 0;

            while (transform.rotation != targetRotation)
            {
                transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / timeToComplete);
                yield return null;

                elapsedTime += Time.deltaTime;
            }
            transform.rotation = targetRotation;
        }

        private void FinishRotation()
        {
            for (int i = 0; i < walkableChild.Count; i++)
            {

            }
        }


    }
}
