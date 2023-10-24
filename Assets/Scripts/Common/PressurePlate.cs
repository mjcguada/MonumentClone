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

    public UnityEvent<Reaction> OnPlatePressedReaction;
    
    public UnityEvent OnPlatePressedEvent;

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
            OnPlatePressedReaction?.Invoke(reaction);
            OnPlatePressedEvent?.Invoke();

            if (!CanBePressedRepeatedly) 
            {
                Destroy(this); // Destroy script component
            }
        }
    }
}
