using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ImageCarousel : MonoBehaviour, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private Scrollbar scrollbarComponent;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private TextMeshProUGUI slideTextReference;
    [SerializeField] private TextMeshProUGUI countTextReference;

    private List<SlideImage> imagesToShow = new List<SlideImage>();

    private float[] positions; // Positions of the elements along the scroll bar
    private float distance; // Distance between elements

    private int currentIndex = 0;
    private bool isSwipping = false;

    private void Start()
    {
        // Fill up carousel elements
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            SlideImage childImage = contentTransform.GetChild(i).GetComponent<SlideImage>();
            if (childImage != null) imagesToShow.Add(childImage);
        }

        UpdateScrollPositions(imagesToShow.Count);

        scrollbarComponent.value = 0; // The start of the scroll rect

        ShowCurrentSlideInfo();
    }

    private void UpdateScrollPositions(int collectionLength)
    {
        currentIndex = 0;

        positions = new float[collectionLength];
        distance = 1f / (positions.Length - 1f);

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = distance * i;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StopAllCoroutines();

        float scrollPosition = scrollbarComponent.value;

        // Find closest element to current scroll value
        float minDistance = float.MaxValue;
        float targetPosition = 0;

        for (int i = 0; i < positions.Length; i++)
        {
            if (Mathf.Abs(positions[i] - scrollPosition) < minDistance)
            {
                minDistance = Mathf.Abs(positions[i] - scrollPosition);
                targetPosition = positions[i];
                currentIndex = i;
            }
        }

        StartCoroutine(SwipeAnimation(targetPosition));
    }

    // Snap to closest element
    IEnumerator SwipeAnimation(float targetPosition)
    {
        isSwipping = true;
        float startingValue = scrollbarComponent.value;
        float timeToComplete = Mathf.Abs(targetPosition - startingValue) * 4.0f;
        float elapsedTime = 0f;

        while (Mathf.Abs(scrollbarComponent.value - targetPosition) > 0.01f)
        {
            // Lerp with "Smoothstep" formula https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
            float t = elapsedTime / timeToComplete;
            t = t * t * (3f - 2f * t);
            scrollbarComponent.value = Mathf.Lerp(scrollbarComponent.value, targetPosition, t);

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        scrollbarComponent.value = targetPosition;
        isSwipping = false;
        ShowCurrentSlideInfo();
    }

    // Called from the arrow buttons attached to the swipe menu
    public void GoToNextElement()
    {
        if (isSwipping) return;

        if (currentIndex < positions.Length - 1)
        {
            currentIndex++;
            float targetPosition = positions[currentIndex];

            StartCoroutine(SwipeAnimation(targetPosition));
        }
    }

    // Called from the arrow buttons attached to the swipe menu
    public void GoToPreviousElement()
    {
        if (isSwipping) return;

        if (currentIndex > 0)
        {
            currentIndex--;
            float targetPosition = positions[currentIndex];

            StartCoroutine(SwipeAnimation(targetPosition));
        }
    }

    private void ShowCurrentSlideInfo()
    {
        if (imagesToShow.Count > currentIndex)
        {
            slideTextReference.text = imagesToShow[currentIndex].SlideInfo;
            countTextReference.text = $"{currentIndex + 1}/{imagesToShow.Count}";
        }
    }
}