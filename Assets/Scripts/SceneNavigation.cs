using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour
{
    public void LoadMenu()
    {
        Debug.Log("SceneNavigation: Attempting to load Main Menu (Index 0)...");
        Time.timeScale = 1f; // Ensure time is unpaused
        // Load Scene Index 0 (Menu)
        SceneManager.LoadScene(0);
    }

    public void ReloadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
