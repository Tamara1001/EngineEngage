using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public int vida = 100;

    public int balasPorCargador = 6;
    public int cargadoresMaximos = 4;

    public int balasActuales;
    public int cargadoresActuales;

    private bool recargando = false;


    private MyWeapon MyWeapon;
    public TextMeshProUGUI vidaText;
    public TextMeshProUGUI recargandoText;
    public TextMeshProUGUI balasText;
    public TextMeshProUGUI cargadoresText;

    public GameObject panelDerrota;

    void Start()
    {

        MyWeapon = FindFirstObjectByType<MyWeapon>();
        balasActuales = balasPorCargador;
        cargadoresActuales = cargadoresMaximos;
        ActualizarUI();
    }

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            Disparar();
        }

        if (cargadoresActuales > 0 && !recargando)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Recargar());
            }
        }
    }

    public void Disparar()
    {
        if (balasActuales > 0 && !recargando)
        {
            balasActuales--;
            MyWeapon.Fire();
            ActualizarUI();
        }
    }

    IEnumerator Recargar()
    {
        recargando = true;
        recargandoText.text = "Recargando...";
        yield return new WaitForSeconds(2f);

        if (cargadoresActuales > 0)
        {
            balasActuales = balasPorCargador;
            cargadoresActuales--;
        }
        recargando = false;
        recargandoText.text = "";
        ActualizarUI();
    }

    public void ReloadZone()
    {
        cargadoresActuales = cargadoresMaximos;
        Debug.Log("Llenando cargadores");
        Recargar();
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (vidaText != null)
            vidaText.text = "Vida: " + vida;
        if (balasText != null)
            balasText.text = "Balas: " + balasActuales;
        if (cargadoresText != null)
            cargadoresText.text = "Cargadores: " + cargadoresActuales;
    }


    void Derrota()
    {
        Time.timeScale = 0f;
        panelDerrota.SetActive(true);
    }

    public void ReceiveDamage()
    {
        vida -= 10;
        ActualizarUI();
        if (vida <= 0)
        {
            Derrota();
        }
    }
}
