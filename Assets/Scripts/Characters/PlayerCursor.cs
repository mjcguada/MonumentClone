using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PlayerCursor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image myImage = null;

    [Header("Scale animation settings")]
    [SerializeField] private float startingSize = 0.25f;
    [SerializeField] private float targetSize = 1.25f;
    [SerializeField] private float animationTime = 0.4f;

    [Header("Fade out animation settings")]
    [SerializeField] private float fadeOutTime = 0.2f;

    private void Start()
    {
        myImage.color = Color.clear;
    }

    public void ShowCursor(Vector2 screenPosition)
    {
        transform.position = screenPosition;

        StopAllCoroutines();

        // Play Bubble Animation
        StartCoroutine(AnimationCoroutine());
    }

    private IEnumerator AnimationCoroutine()
    {
        Vector3 startingScale = Vector3.one * startingSize;
        Vector3 targetScale = Vector3.one * targetSize;

        transform.localScale = startingScale;

        // Make cursor visible (without animation)
        myImage.color = Color.white;

        float elapsedTime = 0;

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;

            // "Smootherstep" lerp formula: https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
            float step = elapsedTime / animationTime;
            step = step * step * step * (step * (6f * step - 15f) + 10f);

            transform.localScale = Vector3.Lerp(startingScale, targetScale, step);
            yield return null;
        }

        // Play Fade In Animation
        StartCoroutine(FadeCoroutine(Color.white, Color.clear, fadeOutTime));
    }

    private IEnumerator FadeCoroutine(Color startingColor, Color targetColor, float fadeDuration)
    {
        // Image is invisible
        myImage.color = startingColor;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            myImage.color = Color.Lerp(startingColor, targetColor, elapsedTime / fadeDuration);
            yield return null;
        }
    }
}
