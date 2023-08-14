using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Monument.World
{
    [RequireComponent(typeof(RotationSnapper))]
    public class RotatorHandle : RotatorInput
    {
        [SerializeField] private RotablePlatform platformToRotate;

        private bool isSnapping = false;

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

            snapper.OnSnapFinished = () =>
            {
                isSnapping = false;

                platformToRotate.transform.localRotation = transform.rotation;
                platformToRotate.ApplyConfiguration();
            };
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            platformToRotate.transform.localRotation = transform.rotation;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            isSnapping = true;
        }

        private void Update()
        {
            if (isSnapping)
            {
                platformToRotate.transform.localRotation = transform.rotation;
            }
        }
    }
}