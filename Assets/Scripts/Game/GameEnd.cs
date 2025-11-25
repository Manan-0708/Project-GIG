using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    [Tooltip("Optional scene name to load when the player reaches the goal. If empty, loads build index 0.")]
    public string sceneToLoad = "";

    [Tooltip("Optional delay before changing scene (seconds).")]
    public float delay = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Game Over! You Win!");
        StartCoroutine(LoadSceneDelayed());
    }

    IEnumerator LoadSceneDelayed()
    {
        yield return new WaitForSeconds(delay);

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // try to load by name if it's present in Build Settings
            if (IsSceneInBuildSettings(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
                yield break;
            }
            else
            {
                Debug.LogWarning($"GameEnd: Scene '{sceneToLoad}' not found in __File > Build Settings__. Falling back to build index 0.");
            }
        }

        if (SceneManager.sceneCountInBuildSettings > 0)
            SceneManager.LoadScene(0);
        else
            Debug.LogError("GameEnd: No scenes in Build Settings. Add at least one scene in __File > Build Settings__.");
    }

    bool IsSceneInBuildSettings(string sceneName)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }
}
