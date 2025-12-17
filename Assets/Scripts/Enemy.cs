using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject SFX_Explosion;
    [SerializeField] private int health;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBar;
    private GameManager gameManager;
    private bool isDead = false;
    private Animator zombieAnimator;
    private NavMeshAgent zombieNavMeshAgent;
    [SerializeField] Transform player;

    public enum EnemyType { Zombie, Mummy, Skeleton }
    public EnemyType enemyType;

    public enum EnemyState { Idle, Chase, Attack }
    public EnemyState currentState = EnemyState.Idle;

    [SerializeField] private GameObject coinPrefab;
    [Header("Combat Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint; 
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastAttackTime;

    [SerializeField] private int damage = 10;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int scoreValue = 10;
    private float maxHealth;
    [SerializeField] private bool isRanged = false;
    
    [HideInInspector]
    public EnemySpawner.ZoneType originZone;

    private string deadTrigger = "Death";
    private string walkAnimationParameter = "MoveSpeed";
    private string attackTrigger = "Attack";

    void Start()
    {
        zombieAnimator = GetComponent<Animator>();
        zombieNavMeshAgent = GetComponent<NavMeshAgent>();
        gameManager = FindFirstObjectByType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        maxHealth = health;
        zombieNavMeshAgent.speed = moveSpeed;

        float randomSpeed = Random.Range(0.8f, 1.2f);
        UpdateUI();
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
            Debug.Log("El enemigo ha alcanzado al jugador");

            collision.gameObject.GetComponent<PlayerManager>().ReceiveDamage(damage);
        }
    }
    void Update()
    {
        if (isDead) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Chase:
                ChaseState();
                break;
            case EnemyState.Attack:
                AttackState();
                break;
        }

        if (zombieAnimator != null)
        {
            zombieAnimator.SetFloat(walkAnimationParameter, zombieNavMeshAgent.velocity.magnitude);
        }
    }

    private void IdleState()
    {
        if (Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            currentState = EnemyState.Chase;
        }
    }

    private void ChaseState()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            currentState = EnemyState.Idle;
            zombieNavMeshAgent.SetDestination(transform.position);
        }
        else if (distance < attackRange)
        {
            currentState = EnemyState.Attack;
            zombieNavMeshAgent.SetDestination(transform.position); 
        }
        else
        {
            zombieNavMeshAgent.SetDestination(player.position);
            zombieNavMeshAgent.isStopped = false; 
        }
    }

    private void AttackState()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            currentState = EnemyState.Chase;
            zombieNavMeshAgent.isStopped = false;
            return;
        }

        zombieNavMeshAgent.velocity = Vector3.zero;
        zombieNavMeshAgent.isStopped = true;
        transform.LookAt(player);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    private void PerformAttack()
    {
        if (isRanged && enemyType == EnemyType.Skeleton)
        {
            // Ataque a distancia
            if (projectilePrefab != null)
            {
                Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + transform.forward + Vector3.up * 1.5f;
                Instantiate(projectilePrefab, spawnPos, transform.rotation);
            }
             zombieAnimator.SetTrigger(attackTrigger); 
        }
        else
        {
             // Ataque cuerpo a cuerpo
             zombieAnimator.SetTrigger(attackTrigger);
             
             if (Vector3.Distance(transform.position, player.position) <= attackRange)
             {
                 PlayerManager pm = player.GetComponent<PlayerManager>();
                 if (pm != null)
                 {
                     pm.ReceiveDamage(damage);
                 }
             }
        }
    }


    private void ApplyDamage(Collider collision)
    {
        int damageReceived = collision.gameObject.GetComponent<Damage>().damageAmount;
        health -= damageReceived;
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyHit();

        GameObject newVFX = Instantiate(SFX_Explosion, collision.transform.position, Quaternion.identity);
        Destroy(newVFX, 2f);
        if (health > 0)
        {
            Debug.Log("Salud restante: " + health);
            UpdateUI();
        }
        else if (!isDead)
        {
            Debug.Log("Enemigo asesinado");
            isDead = true;
            gameManager.OnEnemyKilled(originZone);
            zombieAnimator.SetTrigger(deadTrigger);
            zombieNavMeshAgent.isStopped = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            foreach (Collider col in GetComponents<Collider>())
            {
                col.enabled = false;
            }
            healthText.text = "";
            healthBar.fillAmount = 0;
            if (coinPrefab != null)
            {
                GameObject coinObj = Instantiate(coinPrefab, transform.position, Quaternion.identity);
                Coin coinScript = coinObj.GetComponent<Coin>();
                if (coinScript != null)
                {
                    coinScript.scoreAmount = scoreValue;
                }
            }
        }
    }

    private void UpdateUI()
    {
        healthText.text = "Vida: " + health;
        healthBar.fillAmount = health / maxHealth;
    }
}
