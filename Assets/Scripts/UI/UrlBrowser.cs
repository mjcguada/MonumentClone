using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlBrowser : MonoBehaviour
{
    [SerializeField] private string url;

    public void OpenUrl()
    {
        if (string.IsNullOrEmpty(url)) return;

        Application.OpenURL(url);
    }
}
