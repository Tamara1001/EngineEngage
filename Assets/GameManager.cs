using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject panelVictoria;
    public int enemigosParaGanar = 10;
    private int enemigosEliminados = 0;

    public TextMeshProUGUI Kills;

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

    void Victoria()
    {
        Time.timeScale = 0f;
        panelVictoria.SetActive(true);
    }
}
