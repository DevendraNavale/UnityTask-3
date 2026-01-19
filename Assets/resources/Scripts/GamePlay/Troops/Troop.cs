using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TeamComponent))]
public class Troop : MonoBehaviour
{
    /* ───────────────── CONFIG ───────────────── */

    [Header("Troop Data")]
    public TroopType troopType;
    public float maxHealth = 50f;
    public float attackDamage = 10f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1f;

    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Projectile")]
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 15f;

    [Header("Tank Settings")]
    public float tankFireRange = 10f;

    [Header("Targeting")]
    public float scanInterval = 0.5f;
    public float baseAttackDistance = 2.5f;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;

    /* ───────────────── RUNTIME ───────────────── */

    float currentHealth;
    float attackTimer;
    float scanTimer;

    NavMeshAgent agent;
    TeamComponent teamComponent;
    Animator animator;

    BaseHealth targetBase;
    Transform attackPoint;
    Troop currentEnemyTarget;
    HealthBar healthBar;

    bool isDead;

    /* ───────────────── UNITY ───────────────── */

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        teamComponent = GetComponent<TeamComponent>();
        animator = GetComponentInChildren<Animator>();

        agent.speed = moveSpeed;
        agent.acceleration = 12f;
        agent.angularSpeed = 720f;
        agent.autoBraking = true;
    }

    void Start()
    {
        currentHealth = maxHealth;
        AssignTargetBase();
        SetDestinationToBase();
        SpawnHealthBar();
        ResetAnimatorState();   // 🔑 IMPORTANT
    }

    void Update()
    {
        if (isDead || !agent.isOnNavMesh)
            return;

        attackTimer -= Time.deltaTime;
        scanTimer -= Time.deltaTime;

        UpdateMovementAnimation();

        if (troopType == TroopType.Tank)
        {
            HandleTankBehaviour();
            return;
        }

        if (scanTimer <= 0f)
        {
            scanTimer = scanInterval;
            FindClosestEnemyTroop();
        }

        if (currentEnemyTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentEnemyTarget.transform.position);
            if (dist <= attackRange)
            {
                agent.isStopped = true;
                transform.LookAt(currentEnemyTarget.transform);
                AttackTroop();
                return;
            }
        }

        agent.isStopped = false;
        MoveTowardsBase();
    }

    /* ───────────────── ANIMATION ───────────────── */

    void ResetAnimatorState()
    {
        if (!animator) return;

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsDead", false);
    }

    void UpdateMovementAnimation()
    {
        if (!animator) return;

        bool moving = agent.velocity.sqrMagnitude > 0.05f && !agent.isStopped;
        animator.SetBool("IsMoving", moving);

        if (moving)
            animator.SetBool("IsAttacking", false);
    }

    void TriggerAttackAnimation()
    {
        if (!animator) return;

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttacking", true);
    }

    void TriggerDeathAnimation()
    {
        if (!animator) return;

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsDead", true);
    }

    /* ───────────────── TARGETING ───────────────── */

    void AssignTargetBase()
    {
        if (teamComponent.team == Team.Player)
        {
            targetBase = GameObject.FindWithTag("EnemyBase")?.GetComponent<BaseHealth>();
            attackPoint = GameObject.FindWithTag("EnemyAttackPoint")?.transform;
        }
        else
        {
            targetBase = GameObject.FindWithTag("PlayerBase")?.GetComponent<BaseHealth>();
            attackPoint = GameObject.FindWithTag("PlayerAttackPoint")?.transform;
        }
    }

    void FindClosestEnemyTroop()
    {
        Troop[] troops = FindObjectsByType<Troop>(FindObjectsSortMode.None);
        float closest = Mathf.Infinity;
        currentEnemyTarget = null;

        foreach (var t in troops)
        {
            if (!t || t == this) continue;
            if (t.teamComponent.team == teamComponent.team) continue;

            float d = Vector3.Distance(transform.position, t.transform.position);
            if (d < closest)
            {
                closest = d;
                currentEnemyTarget = t;
            }
        }
    }

    /* ───────────────── MOVEMENT ───────────────── */

    void SetDestinationToBase()
    {
        if (attackPoint && agent.isOnNavMesh)
            agent.SetDestination(attackPoint.position);
    }

    void MoveTowardsBase()
    {
        if (!attackPoint || !targetBase) return;

        float d = Vector3.Distance(transform.position, attackPoint.position);
        if (d <= baseAttackDistance)
        {
            agent.isStopped = true;
            transform.LookAt(targetBase.transform);
            AttackBase();
        }
        else if (!agent.hasPath)
            SetDestinationToBase();
    }

    void HandleTankBehaviour()
    {
        if (!attackPoint || !targetBase) return;

        float d = Vector3.Distance(transform.position, attackPoint.position);
        if (d <= tankFireRange)
        {
            agent.isStopped = true;
            transform.LookAt(targetBase.transform);
            AttackBase();
        }
        else if (!agent.hasPath)
            SetDestinationToBase();
    }

    /* ───────────────── COMBAT ───────────────── */

    void AttackTroop()
    {
        if (attackTimer > 0f || !currentEnemyTarget) return;

        attackTimer = attackCooldown;
        TriggerAttackAnimation();

        if (troopType == TroopType.Melee)
            currentEnemyTarget.TakeDamage(attackDamage);
        else
            FireProjectile(currentEnemyTarget.transform);
    }

    void AttackBase()
    {
        if (attackTimer > 0f || !targetBase) return;

        attackTimer = attackCooldown;
        TriggerAttackAnimation();

        if (troopType == TroopType.Melee)
            targetBase.TakeDamage(attackDamage);
        else
            FireProjectile(targetBase.transform);
    }

    void FireProjectile(Transform target)
    {
        if (!ProjectilePoolManager.Instance || !projectileSpawnPoint) return;

        Projectile p = ProjectilePoolManager.Instance.GetProjectile(
            troopType == TroopType.Tank ? ProjectileType.Tank : ProjectileType.Archer,
            projectileSpawnPoint.position,
            projectileSpawnPoint.rotation);

        p?.Initialize(target, attackDamage, projectileSpeed);
    }

    /* ───────────────── HEALTH ───────────────── */

    void SpawnHealthBar()
    {
        GameObject canvas = GameObject.FindWithTag("WorldCanvas");
        if (!canvas || !healthBarPrefab) return;

        GameObject hb = Instantiate(healthBarPrefab, canvas.transform);
        healthBar = hb.GetComponent<HealthBar>();
        healthBar.target = transform;
        healthBar.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        healthBar?.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        TriggerDeathAnimation();
        Destroy(gameObject, 2.5f);
    }
}
