using UnityEngine;
using UnityEngine.AI;

/* ─────────────────────────────────────────────
 * AI Aggression State
 * ───────────────────────────────────────────── */
public enum AIAggressionState
{
    Normal,
    Aggressive
}

public class EnemySpawnerVR : MonoBehaviour
{
    /* ───────────────────── CONFIG ───────────────────── */

    [Header("Enemy Troop Prefabs")]
    public GameObject archerPrefab;
    public GameObject knightPrefab;
    public GameObject tankPrefab;

    [Header("Spawn Points")]
    public Transform archerSpawnPoint;
    public Transform knightSpawnPoint;
    public Transform tankSpawnPoint;

    [Header("Enemy Energy")]
    public EnemyEnergySystem enemyEnergy;

    [Header("Energy Costs")]
    public float archerCost = 2f;
    public float knightCost = 3f;
    public float tankCost = 5f;

    [Header("Unit Cap")]
    public int maxEnemiesAlive = 90;

    [Header("Spawn Timing")]
    public float decisionInterval = 2.5f;
    public float initialDelay = 5f;
    [Range(0.3f, 1f)]
    public float spawnSpeedMultiplier = 0.7f;

    [Header("Aggression")]
    public float aggressiveDecisionMultiplier = 0.6f;
    public int aggressiveExtraUnits = 10;

    /* ───────────────────── RUNTIME ───────────────────── */

    private float decisionTimer;
    private float elapsedTime;

    private AIAggressionState aggressionState = AIAggressionState.Normal;
    private BaseHealth enemyBase;

    /* ───────────────────── UNITY ───────────────────── */

    private void Start()
    {
        GameObject baseObj = GameObject.FindWithTag("EnemyBase");
        if (baseObj != null)
            enemyBase = baseObj.GetComponent<BaseHealth>();
    }

    private void Update()
    {
        if (enemyEnergy == null || enemyBase == null)
            return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime < initialDelay)
            return;

        UpdateAggressionState();

        int allowedEnemies = maxEnemiesAlive;
        float interval = decisionInterval;

        if (aggressionState == AIAggressionState.Aggressive)
        {
            allowedEnemies += aggressiveExtraUnits;
            interval *= aggressiveDecisionMultiplier;
        }

        int currentEnemies = CountEnemies();
        if (currentEnemies >= allowedEnemies)
        {
            Debug.Log($"[EnemySpawnerVR] Spawn blocked → Unit cap reached ({currentEnemies}/{allowedEnemies})");
            return;
        }

        decisionTimer += Time.deltaTime * spawnSpeedMultiplier;
        if (decisionTimer < interval)
            return;

        decisionTimer = 0f;
        AttemptStrategicSpawn();
    }

    /* ───────────────────── AGGRESSION ───────────────────── */

    private void UpdateAggressionState()
    {
        float healthRatio = enemyBase.currentHealth / enemyBase.maxHealth;

        aggressionState =
            (healthRatio <= 0.1f)
            ? AIAggressionState.Aggressive
            : AIAggressionState.Normal;
    }

    /* ───────────────────── AI DECISION ───────────────────── */

    private void AttemptStrategicSpawn()
    {
        TroopType choice = DecideNextSpawn();

        switch (choice)
        {
            case TroopType.Tank:
                TrySpawn(tankPrefab, tankSpawnPoint, tankCost);
                break;

            case TroopType.Melee:
                TrySpawn(knightPrefab, knightSpawnPoint, knightCost);
                break;

            case TroopType.Ranged:
                TrySpawn(archerPrefab, archerSpawnPoint, archerCost);
                break;
        }
    }

    private TroopType DecideNextSpawn()
    {
        GetPlayerTroopComposition(out int melee, out int ranged, out int tank);

        if (aggressionState == AIAggressionState.Aggressive &&
            enemyEnergy.currentEnergy >= tankCost)
        {
            return TroopType.Tank;
        }

        if (ranged >= melee && ranged >= tank)
            return TroopType.Tank;

        if (tank > melee)
            return TroopType.Melee;

        return TroopType.Ranged;
    }

    /* ───────────────────── ANALYSIS ───────────────────── */

    private void GetPlayerTroopComposition(out int melee, out int ranged, out int tank)
    {
        melee = ranged = tank = 0;

        Troop[] troops = FindObjectsByType<Troop>(FindObjectsSortMode.None);

        foreach (Troop t in troops)
        {
            if (t == null) continue;

            TeamComponent tc = t.GetComponent<TeamComponent>();
            if (tc == null || tc.team != Team.Player) continue;

            switch (t.troopType)
            {
                case TroopType.Melee: melee++; break;
                case TroopType.Ranged: ranged++; break;
                case TroopType.Tank: tank++; break;
            }
        }
    }

    /* ───────────────────── SPAWN ───────────────────── */

    private void TrySpawn(GameObject prefab, Transform point, float cost)
    {
        if (!enemyEnergy.TrySpend(cost))
        {
            Debug.Log("[EnemySpawnerVR] Spawn blocked → Not enough energy");
            return;
        }

        SpawnEnemy(prefab, point);
    }

    private void SpawnEnemy(GameObject prefab, Transform spawnPoint)
    {
        if (prefab == null || spawnPoint == null)
            return;

        GameObject go = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        if (NavMesh.SamplePosition(go.transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            go.transform.position = hit.position;

        TeamComponent tc = go.GetComponent<TeamComponent>();
        if (tc != null)
            tc.team = Team.Enemy;
    }

    /* ───────────────────── UTIL ───────────────────── */

    private int CountEnemies()
    {
        int count = 0;
        Troop[] troops = FindObjectsByType<Troop>(FindObjectsSortMode.None);

        foreach (Troop t in troops)
        {
            if (t == null) continue;

            TeamComponent tc = t.GetComponent<TeamComponent>();
            if (tc != null && tc.team == Team.Enemy)
                count++;
        }

        return count;
    }
    public void Spawn(UnitData cardData, Troop owner)
{
    Debug.Log($"[EnemySpawnerVR] Card routed: {cardData.unitName} (Day-1 stub)");
    // Enemy still uses AI logic in Update() — DO NOT spawn directly
}

}
