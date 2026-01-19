using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PlayerTroopSpawnerVR : MonoBehaviour
{
    [Header("Energy")]
    public PlayerEnergySystem energySystem;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Deck")]
    public List<UnitData> debugDeck;
    private List<UnitData> activeDeck;

    // ✅ ADD THIS
    public int ActivePlayerUnits { get; private set; }

    private void Start()
    {
        if (SelectedDeck.deck != null && SelectedDeck.deck.Count > 0)
            activeDeck = SelectedDeck.deck;
        else
            activeDeck = debugDeck;

        ActivePlayerUnits = 0;
    }

    // Connected to UI Buttons (0, 1, 2)
    public void SpawnUnit(int deckIndex)
    {
        if (activeDeck == null) return;
        if (deckIndex < 0 || deckIndex >= activeDeck.Count) return;

        UnitData unit = activeDeck[deckIndex];

        if (!energySystem.TrySpend(unit.cost))
            return;

        if (unit.prefab == null)
        {
            Debug.LogError("UnitData prefab missing!");
            return;
        }

        Transform spawnPoint = spawnPoints[deckIndex % spawnPoints.Length];

        GameObject troopGO = Instantiate(
            unit.prefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Snap to NavMesh
        if (NavMesh.SamplePosition(
            troopGO.transform.position,
            out NavMeshHit hit,
            5f,
            NavMesh.AllAreas))
        {
            troopGO.transform.position = hit.position;
        }

        // Ensure Player Team
        TeamComponent tc = troopGO.GetComponent<TeamComponent>();
        if (tc != null)
            tc.team = Team.Player;

        // ✅ INCREMENT COUNT
        ActivePlayerUnits++;
    }
    // ================= PIPELINE ENTRY (Day-1) =================
public void SpawnUnit(UnitData unit)
{
    if (activeDeck == null)
    {
        Debug.LogError("[PlayerSpawner] Active deck is null");
        return;
    }

    int index = activeDeck.IndexOf(unit);

    if (index == -1)
    {
        Debug.LogError($"[PlayerSpawner] Unit {unit.unitName} not found in deck");
        return;
    }

    SpawnUnit(index);
}


    // ✅ OPTIONAL (recommended): call this when unit dies
    public void OnPlayerUnitDestroyed()
    {
        ActivePlayerUnits = Mathf.Max(0, ActivePlayerUnits - 1);
    }
}
