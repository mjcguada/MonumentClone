using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{    
    [Header("Events")]
    [SerializeField] private UnityEvent OnHighlightEnter;
    [SerializeField] private UnityEvent OnHighlightExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHighlightEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHighlightExit?.Invoke();
    }
}
