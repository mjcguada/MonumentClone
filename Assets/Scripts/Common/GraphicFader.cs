using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicFader : MonoBehaviour
{
    [SerializeField] private Graphic graphicElement;
    
    [Header("Start")]
    [SerializeField] private bool fadeInOnStart = false;
    [SerializeField] private bool fadeOutOnStart = false;
    
    [Header("Parameters")]
    [SerializeField] private float timeToFadeIn = 1f;
    [SerializeField] private float timeToFadeOut = 1f;


    private void Start()
    {
        if (fadeInOnStart) FadeIn();
        else if (fadeOutOnStart) FadeOut();
    }

    public void FadeIn()
    {
        graphicElement.CrossFadeAlpha(0, 0, false); // Establish alpha to 0 instantly
        graphicElement.CrossFadeAlpha(1f, timeToFadeIn, false); // Fade along time
    }

    public void FadeOut()
    {
        graphicElement.CrossFadeAlpha(1, 0, false); // Establish alpha to 1 instantly
        graphicElement.CrossFadeAlpha(0f, timeToFadeOut, false); // Fade along time
    }
}
