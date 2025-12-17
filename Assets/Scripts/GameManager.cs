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

    public GameObject desertWall;
    public GameObject snowWall;

    public Train trainController;

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
        if (AudioManager.Instance != null) AudioManager.Instance.PlayBGM();
    }

    void Update()
    {
    }

    public void OnEnemyKilled(EnemySpawner.ZoneType enemyZone)
    {
        // Revisa si el enemigo es de la zona actual
        if (enemyZone == currentZone)
        {
            killCounter++;
            totalKills++;
            UpdateKillUI();

            if (killCounter >= killsRequiredPerZone)
            {
                AdvanceZone();
            }
        }
        else
        {
            totalKills++;
        }
    }

    private void AdvanceZone()
    {
        killCounter = 0;
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayAreaComplete();

        switch (currentZone)
        {
            case EnemySpawner.ZoneType.Plains:
                currentZone = EnemySpawner.ZoneType.Desert;
                if (desertWall != null) desertWall.SetActive(false); // Desbloquar Desierto
                if (trainController != null) trainController.MoveToNextStation();
                ActivateSpawnersForZone(EnemySpawner.ZoneType.Desert);
                break;
            case EnemySpawner.ZoneType.Desert:
                currentZone = EnemySpawner.ZoneType.Snow;
                if (snowWall != null) snowWall.SetActive(false); // Desbloquar Nieve
                if (trainController != null) trainController.MoveToNextStation();
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
            Kills.text = killCounter + " / " + killsRequiredPerZone;
        }
    }

    public void SumarEnemigoEliminado() 
    {
        OnEnemyKilled(currentZone);
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (ScoreText != null)
        {
            ScoreText.text = " " + score;
        }
    }


    void Victory()
    {
        SaveStats(true); // Guarda todo en victoria
        ShowEndGameUI(panelVictoria);
    }

    public void TriggerDefeat()
    {
        SaveStats(false); // No guardar tiempo en derrota
        ShowEndGameUI(panelDerrota);
    }

    private void ShowEndGameUI(GameObject panel)
    {
        float endTime = Time.time - startTime;
        
        // Actualiza UI
        if (finalScoreText != null) finalScoreText.text = "Score: " + score;
        if (finalKillsText != null) finalKillsText.text = "Kills: " + totalKills;
        if (finalTimeText != null) finalTimeText.text = "Time: " + FormatTime(endTime);

        Time.timeScale = 0f;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        DestroyAllActiveEnemies();
        if (panel != null) panel.SetActive(true);
        if (final != null) final.SetActive(true);
    }

    private void SaveStats(bool isVictory)
    {
        float endTime = Time.time - startTime;

        // Guarda Best Score
        int currentBestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (score > currentBestScore)
        {
            PlayerPrefs.SetInt("BestScore", score);
        }

        int currentBestKills = PlayerPrefs.GetInt("BestKills", 0);
        if (totalKills > currentBestKills)
        {
            PlayerPrefs.SetInt("BestKills", totalKills);
        }

        if (isVictory)
        {
            float currentBestTime = PlayerPrefs.GetFloat("BestTime", 999999f);
            if (endTime < currentBestTime)
            {
                PlayerPrefs.SetFloat("BestTime", endTime);
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
