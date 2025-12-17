using UnityEngine;
using System.Collections;
using UnityEngine.UI;
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
    public Image healthBar;
    public TextMeshProUGUI recargandoText;
    public TextMeshProUGUI balasText;
    public TextMeshProUGUI cargadoresText;

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
        Recargar();
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (healthBar != null)
            healthBar.fillAmount = (float)vida / 100f;
        if (balasText != null)
            balasText.text = " " + balasActuales;
        if (cargadoresText != null)
            cargadoresText.text = " " + cargadoresActuales;
    }

    void Derrota()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.TriggerDefeat();
        }
    }

    public void ReceiveDamage(int damageAmount)
    {
        vida -= damageAmount;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayPlayerDamage();
        ActualizarUI();
        if (vida <= 0)
        {
            Derrota();
        }
    }
}
