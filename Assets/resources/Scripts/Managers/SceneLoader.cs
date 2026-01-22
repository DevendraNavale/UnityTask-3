using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Load a scene by name
    public void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}...");
        SceneManager.LoadScene(sceneName);
    }

    // Reload current scene (Play Again)
    public void ReloadCurrentScene()
    {
        Debug.Log("Reloading Match...");

        // Small delay so log appears before scene reload
        Invoke(nameof(DoReload), 0.1f);
    }

    private void DoReload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Quit the game / exit application
    public void QuitGame()
    {
        Debug.Log("Quit Game called.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
