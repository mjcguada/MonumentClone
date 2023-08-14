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
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {

        }

        public override void OnDrag(PointerEventData eventData)
        {

        }

        public override void OnEndDrag(PointerEventData eventData)
        {

        }
    }
}