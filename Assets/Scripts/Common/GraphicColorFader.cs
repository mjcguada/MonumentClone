using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GraphicColorFader : MonoBehaviour
{
    [SerializeField] private Graphic graphicReference;
    [SerializeField] private Color colorToFade = Color.yellow;
    [SerializeField] private float timeToFade = 1f;

    private Color originalColor = default;
    private Coroutine fadeCoroutine = null;

    private void Awake()
    {
#if UNITY_EDITOR
        if (graphicReference == null)
        {
            Debug.LogError("The Graphic reference is null", gameObject);
            Destroy(this); return;
        }
#endif
        originalColor = graphicReference.color;
    }

    public void FadeIn()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeColorCoroutine(colorToFade));
    }

    public void FadeOut()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeColorCoroutine(originalColor));
    }

    private IEnumerator FadeColorCoroutine(Color targetColor)
    {
        float elapsedTime = 0;
        Color startingColor = graphicReference.color;

        while (elapsedTime < timeToFade)
        {
            graphicReference.color = Color.Lerp(startingColor, targetColor, elapsedTime / timeToFade);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
