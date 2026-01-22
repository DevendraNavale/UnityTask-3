using UnityEngine;
using System.Collections.Generic;

public class UnitRegistry : MonoBehaviour
{
    public int GetPlayerAlive() => playerUnits.Count;
public int GetEnemyAlive() => enemyUnits.Count;

    private readonly List<GameObject> playerUnits = new();
    private readonly List<GameObject> enemyUnits = new();
    private int nextUnitId = 1;

    public int GetNextUnitId() => nextUnitId++;

    public void Register(GameObject unit, Team team)
    {
        if (team == Team.Player) playerUnits.Add(unit);
        else enemyUnits.Add(unit);
    }

    public void Unregister(GameObject unit, Team team)
    {
        if (team == Team.Player) playerUnits.Remove(unit);
        else enemyUnits.Remove(unit);
    }

    public int GetAliveCount(Team team)
    {
        return team == Team.Player ? playerUnits.Count : enemyUnits.Count;
    }

    public int GetTotalAlive()
    {
        return playerUnits.Count + enemyUnits.Count;
    }
}
