using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Reaction reaction;
    [Space]

    public bool CanBePressedRepeatedly = false;

    public UnityEvent<Reaction> OnPlatePressed;

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
            OnPlatePressed?.Invoke(reaction);

            if (!CanBePressedRepeatedly) 
            {
                Destroy(this); // Destroy script component
            }
        }
    }
}
