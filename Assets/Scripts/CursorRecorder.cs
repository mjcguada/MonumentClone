using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorRecorder : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.SetCursor(cursorTexture, Vector2.one * 0.4f, CursorMode.ForceSoftware);
        }
        else
        {
            // Default cursor
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
