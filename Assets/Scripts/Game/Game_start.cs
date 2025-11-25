using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_start : MonoBehaviour
{
    [Tooltip("Scene name to load when Start button is pressed. If empty, the next build index will be loaded.")]
    public string sceneToLoad;

    [Tooltip("Optional delay before loading the scene (seconds).")]
    public float startDelay = 0f;

    // Called by Start button (assign this in the Button -> OnClick)
    public void StartGame()
    {
        if (startDelay <= 0f)
            LoadTargetScene();
        else
            StartCoroutine(StartDelayed());
    }

    IEnumerator StartDelayed()
    {
        yield return new WaitForSeconds(startDelay);
        LoadTargetScene();
    }

    void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
            return;
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("Game_start: No scene specified and next build index is out of range.");
        }
    }

    // Called by Quit button (assign this in the Button -> OnClick)
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Stop play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
