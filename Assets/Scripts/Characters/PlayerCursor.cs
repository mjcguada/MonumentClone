using Monument.World;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PlayerCursor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image myImage = null;
    [SerializeField] private LayerMask navNodeLayerMask;

    [Header("Scale animation settings")]
    [SerializeField] private float startingSize = 0.25f;
    [SerializeField] private float targetSize = 1.25f;
    [SerializeField] private float animationTime = 0.4f;

    [Header("Fade out animation settings")]
    [SerializeField] private float fadeOutTime = 0.2f;

    private MonumentInput inputActions;

    private void Start()
    {
        myImage.color = Color.clear;
    }

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new MonumentInput();
            inputActions.Player.Click.performed += ShowCursor;
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions?.Disable();
    }

    private void ShowCursor(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(inputVector);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, navNodeLayerMask))
        {
            // Play the animation if what the user clicks is a NavNode
            NavNode target = hit.transform.gameObject.GetComponent<NavNode>();
            if (target == null) return;

            // Move cursor texture to NavNode position projected on screen
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(target.WalkPoint);
            transform.position = screenPosition;

            // Stop previous animation
            StopAllCoroutines();

            // Play bubble animation
            StartCoroutine(AnimationCoroutine());
        }
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
        StartCoroutine(FadeOutCoroutine(Color.white, Color.clear, fadeOutTime));
    }

    private IEnumerator FadeOutCoroutine(Color startingColor, Color targetColor, float fadeDuration)
    {
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
