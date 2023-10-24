using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneField previousScene;
    [SerializeField] private SceneField nextScene;

    public void LoadPreviousScene()
    {
        LoadScene(previousScene);
    }

    public void LoadNextScene() 
    {
        LoadScene(nextScene);
    }

    private void LoadScene(string scene) 
    {
        if (string.IsNullOrEmpty(scene)) return;

        SceneManager.LoadScene(scene);
    }
}
