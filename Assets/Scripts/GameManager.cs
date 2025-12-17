using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject panelVictoria;
    public int enemigosParaGanar = 10;
    private int enemigosEliminados = 0;
    private int score = 0;

    public TextMeshProUGUI Kills;
    public TextMeshProUGUI ScoreText;

    void Update()
    {

        if (enemigosEliminados >= enemigosParaGanar)
        {
            Victoria();
        }
    }

    public void SumarEnemigoEliminado()
    {
        enemigosEliminados++;
        Kills.text = "Kills: " + enemigosEliminados + "/" + enemigosParaGanar;
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (ScoreText != null)
        {
            ScoreText.text = "Score: " + score;
        }
    }

    void Victoria()
    {
        Time.timeScale = 0f;
        panelVictoria.SetActive(true);
    }
}
