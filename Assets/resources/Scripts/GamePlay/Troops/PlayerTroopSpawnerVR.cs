using UnityEngine;
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

    [Header("Engine")]
    public MatchEngine matchEngine;

    private void Start()
    {
        if (SelectedDeck.deck != null && SelectedDeck.deck.Count > 0)
        {
            activeDeck = SelectedDeck.deck;
            Debug.Log("[PlayerSpawner] Using SelectedDeck");
        }
        else
        {
            activeDeck = debugDeck;
            Debug.Log("[PlayerSpawner] Using DebugDeck");
        }

        if (matchEngine == null)
            Debug.LogError("[PlayerSpawner] MatchEngine NOT assigned");

        if (energySystem == null)
            Debug.LogError("[PlayerSpawner] EnergySystem NOT assigned");

        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogError("[PlayerSpawner] SpawnPoints NOT assigned");
    }

    // ---------------- UI BUTTON ENTRY ----------------
    public void SpawnUnit(int deckIndex)
    {
        Debug.Log($"[PlayerSpawner] SpawnUnit called | index={deckIndex}");

        if (activeDeck == null)
        {
            Debug.LogError("[PlayerSpawner] ActiveDeck is NULL");
            return;
        }

        if (deckIndex < 0 || deckIndex >= activeDeck.Count)
        {
            Debug.LogError("[PlayerSpawner] Invalid deck index");
            return;
        }

        if (matchEngine == null)
        {
            Debug.LogError("[PlayerSpawner] MatchEngine is NULL");
            return;
        }

        // 🔒 UNIT CAP GUARD (O(1), NO SEARCH)
        if (matchEngine.ActiveUnitCount >= matchEngine.MAX_UNITS)
        {
            Debug.Log("[GUARD] Spawn rejected — unit cap reached");
            return;
        }

        UnitData unit = activeDeck[deckIndex];

        if (unit == null)
        {
            Debug.LogError("[PlayerSpawner] UnitData is NULL");
            return;
        }

        if (energySystem == null || !energySystem.TrySpend(unit.cost))
        {
            Debug.Log("[PlayerSpawner] Spawn blocked: Not enough energy");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[PlayerSpawner] No spawn points available");
            return;
        }

        Transform spawnPoint = spawnPoints[deckIndex % spawnPoints.Length];

        Debug.Log($"[PlayerSpawner] Requesting spawn: {unit.unitName}");

        // 🔑 CENTRALIZED SPAWN
        matchEngine.RequestSpawn(
            unit,
            Team.Player,
            spawnPoint.position,
            SpawnReason.CardTap
        );
    }

    // ---------------- PIPELINE ENTRY (UnitData) ----------------
    public void SpawnUnit(UnitData unit)
    {
        if (unit == null)
        {
            Debug.LogError("[PlayerSpawner] UnitData is NULL");
            return;
        }

        if (activeDeck == null)
        {
            Debug.LogError("[PlayerSpawner] ActiveDeck is NULL");
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
}
