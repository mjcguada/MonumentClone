using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraColorFader : MonoBehaviour
{
    [SerializeField] private Camera cameraComponent;
    [SerializeField] private Color targetColor = Color.yellow;

    [SerializeField] private float timeToFade = 1f;

    private Coroutine fadeCoroutine = null;

#if UNITY_EDITOR
    private void Awake()
    {
        if (cameraComponent == null) 
        {
            Debug.LogError("The Camera Component reference is null", gameObject);
            Destroy(this); return;
        }
    }
#endif

    public void Fade()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeColorCoroutine());
    }

    private IEnumerator FadeColorCoroutine()
    {
        float elapsedTime = 0;
        Color startingColor = cameraComponent.backgroundColor;

        while (elapsedTime < timeToFade)
        {
            cameraComponent.backgroundColor = Color.Lerp(startingColor, targetColor, elapsedTime / timeToFade);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }

}
