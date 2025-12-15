using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class ZombieEnemy : MonoBehaviour
{
    [SerializeField] private GameObject SFX_Explosion;
    [SerializeField] private int health = 100;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBar;
    private GameManager gameManager;
    private bool isDead = false;
    private Animator zombieAnimator;
    private NavMeshAgent zombieNavMeshAgent;
    [SerializeField] Transform player;
    [SerializeField] float chaseInterval = 0.5f;

    private string deadTrigger = "Death";
    private string walkAnimationParameter = "MoveSpeed";
    void Start()
    {
        UpdateUI();
        zombieAnimator = GetComponent<Animator>();
        zombieNavMeshAgent = GetComponent<NavMeshAgent>();
        gameManager = FindFirstObjectByType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating(nameof(SetDestination), 3f, chaseInterval);
        float randomSpeed = Random.Range(0.8f, 1.2f);
        zombieNavMeshAgent.speed = randomSpeed;
        zombieAnimator.speed = randomSpeed;

    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            ApplyDamage(collision);
        }

        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            Debug.Log("El zombie ha alcanzado al jugador");

            collision.gameObject.GetComponent<PlayerManager>().ReceiveDamage();
        }
    }
    void Update()
    {
        if (zombieAnimator != null)
        {
            zombieAnimator.SetFloat(walkAnimationParameter, zombieNavMeshAgent.velocity.magnitude);
        }
    }
    private void SetDestination()
    {
        zombieNavMeshAgent.SetDestination(player.position);
    }


    private void ApplyDamage(Collider collision)
    {
        int damageReceived = collision.gameObject.GetComponent<Damage>().damageAmount;
        health -= damageReceived;
        GameObject newVFX = Instantiate(SFX_Explosion, collision.transform.position, Quaternion.identity);
        Destroy(newVFX, 2f);
        if (health > 0)
        {
            Debug.Log("Salud restante: " + health);
            UpdateUI();
        }
        else if (!isDead)
        {
            Debug.Log("Zombie asesinado");
            isDead = true;
            gameManager.SumarEnemigoEliminado();
            zombieAnimator.SetTrigger(deadTrigger);
            zombieNavMeshAgent.isStopped = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            healthText.text = "";
            healthBar.fillAmount = 0;
            //Destroy(gameObject);
        }
    }

    private void UpdateUI()
    {
        healthText.text = "Vida: " + health;
        healthBar.fillAmount = health / 100f;
    }
}
