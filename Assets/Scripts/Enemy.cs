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
    [SerializeField] float chaseInterval = 0.5f;

    public enum EnemyType { Zombie, Mummy, Skeleton }
    public EnemyType enemyType;

    public enum EnemyState { Idle, Chase, Attack }
    public EnemyState currentState = EnemyState.Idle;

    [SerializeField] private GameObject coinPrefab; // Coin prefab to drop
    [Header("Combat Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint; // Optional point to fire from
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastAttackTime;

    private int damage;
    private float maxHealth;
    private bool isRanged = false;


    private string deadTrigger = "Death";
    private string walkAnimationParameter = "MoveSpeed";
    private string attackTrigger = "Attack"; // Assuming an attack trigger exists or will use boolean

    void Start()
    {
        zombieAnimator = GetComponent<Animator>();
        zombieNavMeshAgent = GetComponent<NavMeshAgent>();
        gameManager = FindFirstObjectByType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        SetupEnemy();
        float randomSpeed = Random.Range(0.8f, 1.2f);
        UpdateUI();
        //zombieNavMeshAgent.speed = randomSpeed; // Overridden by SetupEnemy
        zombieAnimator.speed = randomSpeed;
    }

    private void SetupEnemy()
    {
        switch (enemyType)
        {
            case EnemyType.Zombie:
                health = 50;
                damage = 10;
                zombieNavMeshAgent.speed = 2;
                break;
            case EnemyType.Mummy:
                health = 100;
                damage = 30;
                zombieNavMeshAgent.speed = 1;
                break;
            case EnemyType.Skeleton:
                health = 30;
                damage = 20;
                zombieNavMeshAgent.speed = 3;
                isRanged = true;
                break;
        }
        maxHealth = health;
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
            zombieNavMeshAgent.SetDestination(transform.position); // Stop moving
        }
        else if (distance < attackRange)
        {
            currentState = EnemyState.Attack;
            zombieNavMeshAgent.SetDestination(transform.position); // Stop moving
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

        // Stop moving/Keep stopped
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
            // Ranged Attack
            if (projectilePrefab != null)
            {
                // Use firePoint if available, else somewhat in front and UP
                Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + transform.forward + Vector3.up * 1.5f;
                Instantiate(projectilePrefab, spawnPos, transform.rotation);
            }
            // Trigger animation if available
             zombieAnimator.SetTrigger(attackTrigger); 
        }
        else
        {
             // Melee Attack
             zombieAnimator.SetTrigger(attackTrigger);
             
             // Apply damage directly if in range
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
            gameManager.SumarEnemigoEliminado();
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
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }
            //Destroy(gameObject);
        }
    }

    private void UpdateUI()
    {
        healthText.text = "Vida: " + health;
        healthBar.fillAmount = health / maxHealth;
    }
}
