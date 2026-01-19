using UnityEngine;

public class MatchEngine : MonoBehaviour
{
    [Header("Core Systems")]
    public CardExecutionRouter clerk;     // Assign in Inspector
    public RuleSystem ruleSystem;          // Assign in Inspector

   public void RequestCardPlay(UnitData cardData, Troop player)
{
    Debug.Log($"[Gate] Card requested: {cardData.unitName}");

    if (ruleSystem != null)
    {
        if (!ruleSystem.CanPlayCard(cardData))
        {
            Debug.Log("[Gate] Card play blocked by RuleSystem");
            return;
        }

        Debug.Log("[Gate] Card approved by RuleSystem");
    }

    if (clerk == null)
    {
        Debug.LogError("[Gate] Clerk not assigned");
        return;
    }

    clerk.Execute(cardData, player);
}

    #if UNITY_EDITOR
[ContextMenu("TEST / Play First Card")]
private void TestPlayCard()
{
    if (ruleSystem == null || clerk == null)
    {
        Debug.LogError("[TEST] RuleSystem or Clerk missing");
        return;
    }

    if (ruleSystem.playerSpawner == null)
    {
        Debug.LogError("[TEST] PlayerSpawner missing in RuleSystem");
        return;
    }

    UnitData testCard = ruleSystem.playerSpawner.debugDeck[0];
    Debug.Log("[TEST] Triggering test card play");

    RequestCardPlay(testCard, null);
}
#endif

}
