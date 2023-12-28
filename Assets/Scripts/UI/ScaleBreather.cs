using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleBreather : MonoBehaviour
{
    [SerializeField] private Graphic graphicElement;

    [SerializeField] private float animationSpeed = 1.0f;
    [SerializeField] private float amplitude = 1.0f;

    private void Start()
    {
#if UNITY_EDITOR
        if (graphicElement == null)
        {
            Debug.LogError("Graphic element reference is null");
            Destroy(this);
            return;
        }
#endif
    }

    void Update()
    {
        // Calculate the sine value based on time
        float sineScale = Mathf.Sin(Time.time * animationSpeed) * amplitude;

        // Apply the sine value to the GameObject's scale
        transform.localScale = new Vector3(1.0f + sineScale, 1.0f + sineScale, 1.0f + sineScale);
    }
}
