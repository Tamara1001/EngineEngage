using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject panelVictoria;
    public GameObject panelDerrota;
    public GameObject final;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalKillsText;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI Kills;
    public TextMeshProUGUI ScoreText;

    public int killsRequiredPerZone = 3;
    private int killCounter = 0;
    public EnemySpawner.ZoneType currentZone = EnemySpawner.ZoneType.Plains;
    private int score = 0;

    private float startTime;
    private int totalKills;

    void Start()
    {
        startTime = Time.time;
        ActivateSpawnersForZone(currentZone);
        UpdateKillUI();
    }

    void Update()
    {
        // Removed old Update check
    }

    public void OnEnemyKilled(EnemySpawner.ZoneType enemyZone)
    {
        Debug.Log($"GameManager: Enemy Died. Origin Zone: {enemyZone}, Current Zone: {currentZone}");
        
        // Only count if the enemy belongs to the current zone
        if (enemyZone == currentZone)
        {
            killCounter++;
            totalKills++; // Move total kills inside? Or keep it global? 
                          // User requested "To advance... must kill 3 enemies in that zone."
                          // Total kills usually tracks eveything. I will keep totalKills global 
                          // but killCounter specific to zone validation.
            UpdateKillUI();

            if (killCounter >= killsRequiredPerZone)
            {
                AdvanceZone();
            }
        }
        else
        {
            // Still count for total stats maybe?
            totalKills++;
        }
    }

    private void AdvanceZone()
    {
        killCounter = 0;
        
        switch (currentZone)
        {
            case EnemySpawner.ZoneType.Plains:
                currentZone = EnemySpawner.ZoneType.Desert;
                ActivateSpawnersForZone(EnemySpawner.ZoneType.Desert);
                break;
            case EnemySpawner.ZoneType.Desert:
                currentZone = EnemySpawner.ZoneType.Snow;
                ActivateSpawnersForZone(EnemySpawner.ZoneType.Snow);
                break;
            case EnemySpawner.ZoneType.Snow:
                Victory();
                break;
        }
        UpdateKillUI();
    }

    private void ActivateSpawnersForZone(EnemySpawner.ZoneType zone)
    {
        EnemySpawner[] spawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (EnemySpawner spawner in spawners)
        {
            if (spawner.zoneType == zone)
            {
                spawner.ActivateSpawner();
            }
        }
    }

    private void UpdateKillUI()
    {
        if (Kills != null)
        {
            Kills.text = "Enemies Killed: " + killCounter + " / " + killsRequiredPerZone;
        }
    }

    // Keep compatibility for old calls if strictly needed, or remove if we update Enemy.cs
    public void SumarEnemigoEliminado() 
    {
        // Default to current to default behavior if called from elsewhere? 
        // Or better, assume it counts. But safely: logic requires zone.
        // Let's assume Plains for legacy calls or just log warning.
        OnEnemyKilled(currentZone);
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log($"GameManager: Score updated. Current Score: {score}");
        if (ScoreText != null)
        {
            ScoreText.text = "Score: " + score;
        }
    }


    void Victory()
    {
        SaveStats(true); // Save Time only on Victory
        ShowEndGameUI(panelVictoria);
    }

    public void TriggerDefeat()
    {
        SaveStats(false); // Do not save Time on Defeat
        ShowEndGameUI(panelDerrota);
    }

    private void ShowEndGameUI(GameObject panel)
    {
        float endTime = Time.time - startTime;
        
        // Update UI
        if (finalScoreText != null) finalScoreText.text = "Score: " + score;
        if (finalKillsText != null) finalKillsText.text = "Kills: " + totalKills;
        if (finalTimeText != null) finalTimeText.text = "Time: " + FormatTime(endTime);

        Time.timeScale = 0f;
        
        // Unlock Cursor for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        DestroyAllActiveEnemies();
        if (panel != null) panel.SetActive(true);
        if (final != null) final.SetActive(true);
    }

    private void SaveStats(bool isVictory)
    {
        float endTime = Time.time - startTime;

        // Save Best Score (Higher is better) - Always save
        int currentBestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (score > currentBestScore)
        {
            PlayerPrefs.SetInt("BestScore", score);
            Debug.Log("GameManager: New Best Score Saved!");
        }

        // Save Best Kills (Higher is better) - Always save
        int currentBestKills = PlayerPrefs.GetInt("BestKills", 0);
        if (totalKills > currentBestKills)
        {
            PlayerPrefs.SetInt("BestKills", totalKills);
             Debug.Log("GameManager: New Best Kills Saved!");
        }

        // Save Best Time (Lower is better) - ONLY if Victory
        if (isVictory)
        {
            float currentBestTime = PlayerPrefs.GetFloat("BestTime", 999999f);
            if (endTime < currentBestTime)
            {
                PlayerPrefs.SetFloat("BestTime", endTime);
                Debug.Log("GameManager: New Best Time Saved!");
            }
        }

        PlayerPrefs.Save();
        Debug.Log("GameManager: PlayerPrefs Saved.");
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void DestroyAllActiveEnemies()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }
}
