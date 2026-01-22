using UnityEngine;

public class MatchEngine : MonoBehaviour
{
    public static MatchEngine Instance;

    [Header("Registry")]
    public UnitRegistry unitRegistry;

    private int matchIdCounter = 0;
    private bool matchRunning;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // =================== MATCH ===================
    public void StartMatch()
    {
        matchRunning = true;
        matchIdCounter++;

        Debug.Log(
            $"MATCH_START | id={matchIdCounter} | time={Time.time}"
        );
    }

    public void EndMatch(Team winner, string reason)
    {
        matchRunning = false;

        Debug.Log(
            $"MATCH_END | id={matchIdCounter} | winner={winner} | reason={reason} " +
            $"| alive_player={unitRegistry.GetPlayerAlive()} " +
            $"| alive_enemy={unitRegistry.GetEnemyAlive()} " +
            $"| time={Time.time}"
        );
    }

    // =================== UNIT SPAWN ===================
    public void RequestSpawn(UnitData unitData, Team team, Vector3 position, SpawnReason reason)
    {
        if (!matchRunning)
        {
            Debug.LogWarning("[MatchEngine] Spawn blocked: match not running");
            return;
        }

        if (unitData == null || unitData.prefab == null)
        {
            Debug.LogError("[MatchEngine] Spawn failed: UnitData or prefab missing");
            return;
        }

        GameObject unitGO = Instantiate(unitData.prefab, position, Quaternion.identity);

        // Snap to NavMesh
        if (UnityEngine.AI.NavMesh.SamplePosition(
            unitGO.transform.position,
            out var hit,
            5f,
            UnityEngine.AI.NavMesh.AllAreas))
        {
            unitGO.transform.position = hit.position;
        }

        // Team
        TeamComponent tc = unitGO.GetComponent<TeamComponent>();
        if (tc != null)
            tc.team = team;

        // Identity
        UnitIdentity identity = unitGO.GetComponent<UnitIdentity>();
        if (identity != null)
        {
            identity.unitId = unitRegistry.GetNextUnitId();
            identity.cardSource = unitData.unitName;
        }

        // Register
        unitRegistry.Register(unitGO, team);

        // ✅ Explainable log
        Debug.Log(
            $"UNIT_SPAWN | id={identity?.unitId} | unit={unitData.unitName} | team={team} " +
            $"| alive_player={unitRegistry.GetPlayerAlive()} " +
            $"| alive_enemy={unitRegistry.GetEnemyAlive()} " +
            $"| reason={reason}"
        );
    }

    // =================== UNIT DAMAGE ===================
    public void LogUnitDamage(UnitIdentity attacker, UnitIdentity target, float damage)
    {
        if (attacker == null || target == null) return;

        Debug.Log(
            $"UNIT_DAMAGE | attackerId={attacker.unitId} | targetId={target.unitId} | dmg={damage}"
        );
    }

    // =================== UNIT DEATH ===================
    public void LogUnitDeath(UnitIdentity unit)
    {
        if (unit == null) return;

        TeamComponent tc = unit.GetComponent<TeamComponent>();
        Team team = tc != null ? tc.team : Team.Player;

        unitRegistry.Unregister(unit.gameObject, team);

        Debug.Log(
            $"UNIT_DEATH | id={unit.unitId} | unit={unit.cardSource} | team={team} " +
            $"| alive_player={unitRegistry.GetPlayerAlive()} " +
            $"| alive_enemy={unitRegistry.GetEnemyAlive()}"
        );
    }
}
