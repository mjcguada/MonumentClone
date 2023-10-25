using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script attached to gameObjects that are part of
 * an ImageCarousel. Containing an Image and public string
 * to show info
 */

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class SlideImage : MonoBehaviour
{
    public string SlideInfo = "";
}
