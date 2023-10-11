using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LevelEndingTrigger : MonoBehaviour
{
    [SerializeField] private GameEvent levelEndingEvent;

    private void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        IPresser presserComponent = other.GetComponent<IPresser>();
        if (presserComponent != null)
        {
            levelEndingEvent?.Raise();

            Destroy(this); // Destroy script component
        }
    }
}
