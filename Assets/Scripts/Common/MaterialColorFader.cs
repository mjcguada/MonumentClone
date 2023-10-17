using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialColorFader : MonoBehaviour
{
    [SerializeField] private Renderer rendererReference;
    [SerializeField] private Color targetColor = Color.yellow;

    [SerializeField] private float timeToFade = 1f;

    private Coroutine fadeCoroutine = null;
    private Material materialReference;

    private void Awake()
    {
#if UNITY_EDITOR
        if (rendererReference == null)
        {
            Debug.LogError("The Material reference is null", gameObject);
            Destroy(this); return;
        }
#endif
        materialReference = rendererReference.material;
    }

    public void Fade()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeColorCoroutine());
    }

    private IEnumerator FadeColorCoroutine()
    {
        float elapsedTime = 0;
        Color startingColor = materialReference.color;

        while (elapsedTime < timeToFade)
        {
            elapsedTime += Time.deltaTime;
            materialReference.color = Color.Lerp(startingColor, targetColor, elapsedTime / timeToFade);
            yield return null;
        }
    }
}
