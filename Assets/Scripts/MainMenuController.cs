using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI bestKillsText;
    public TextMeshProUGUI bestTimeText;

    void Start()
    {
        // Load Score
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (bestScoreText != null) bestScoreText.text = "Mejor Puntaje: " + bestScore;

        // Load Kills
        int bestKills = PlayerPrefs.GetInt("BestKills", 0);
        if (bestKillsText != null) bestKillsText.text = "Kills Maximas: " + bestKills;

        // Load Time
        float bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        if (bestTimeText != null)
        {
            if (bestTime >= 999999f || bestTime == 0f)
                bestTimeText.text = "Tiempo Record: --:--";
            else
                bestTimeText.text = "Tiempo Record: " + FormatTime(bestTime);
        }
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        // Load Scene Index 1 (Game)
        SceneManager.LoadScene(1);
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
