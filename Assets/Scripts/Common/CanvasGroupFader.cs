using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Start")]
    [SerializeField] private bool fadeInOnStart = false;
    [SerializeField] private bool fadeOutOnStart = false;

    [Header("Parameters")]
    [SerializeField] private float timeToFadeIn = 1f;
    [SerializeField] private float timeToFadeOut = 1f;

    private Coroutine coroutine;

    private void Start()
    {
        if (fadeInOnStart) FadeIn();
        else if (fadeOutOnStart) FadeOut();
    }

    public void FadeIn()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(FadeCoroutine(0, 1, timeToFadeIn));
    }

    public void FadeOut()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(FadeCoroutine(1, 0, timeToFadeOut));
    }

    private IEnumerator FadeCoroutine(float startingAlpha, float targetAlpha, float timeToFade)
    {
        float elapsedTime = 0;
        while (elapsedTime < timeToFade)
        {
            canvasGroup.alpha = Mathf.Lerp(startingAlpha, targetAlpha, elapsedTime / timeToFade);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}