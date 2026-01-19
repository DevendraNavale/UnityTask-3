using UnityEngine;

public class CardExecutionRouter : MonoBehaviour
{
    public PlayerTroopSpawnerVR playerSpawner;
    public EnemySpawnerVR enemySpawner;

    public void Execute(UnitData cardData, Troop owner)
    {
        Debug.Log($"[Clerk] Routing card: {cardData.unitName}");

        if (owner == null)
        {
            Debug.LogError("[Clerk] Owner is null");
            return;
        }

        TeamComponent tc = owner.GetComponent<TeamComponent>();

        if (tc != null && tc.team == Team.Player)
        {
            if (playerSpawner != null)
            {
                playerSpawner.SpawnUnit(cardData);
            }
        }
        else
        {
            if (enemySpawner != null)
            {
                enemySpawner.Spawn(cardData, owner); // Day-1 stub
            }
        }
    }
}
