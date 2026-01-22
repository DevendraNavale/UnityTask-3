using UnityEngine;
using System.Collections.Generic;

public class RuleSystem : MonoBehaviour
{
    [Header("References")]
    public PlayerTroopSpawnerVR playerSpawner;

    [Header("Limits")]
    public int maxPlayerUnits = 5;

    private Dictionary<UnitData, float> cooldownTracker = new();

    public bool CanPlayCard(UnitData card)
    {
        // 1️⃣ Null safety
        if (card == null)
        {
            Debug.Log("[RULE] Card is null");
            return false;
        }

        // 2️⃣ Energy validation
        PlayerEnergySystem energy = FindFirstObjectByType<PlayerEnergySystem>();
        if (energy == null || energy.currentEnergy < card.cost)
        {
            Debug.Log("[RULE] Card blocked: Not enough energy");
            return false;
        }
        Debug.Log("[RULE] Energy OK");

        // 3️⃣ Army cap validation
        if (MatchEngine.Instance != null &&
            MatchEngine.Instance.unitRegistry.GetAliveCount(Team.Player) >= maxPlayerUnits)
        {
            Debug.Log("[RULE] Card blocked: Army cap reached");
            return false;
        }
        Debug.Log("[RULE] Army cap OK");

        return true;
    }
}
