using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitIdentity identity;
    public float health = 100;

    public void TakeDamage(float dmg, Unit attacker)
    {
        health -= dmg;

        // Log damage
        MatchEngine.Instance.LogUnitDamage(attacker.identity, identity, dmg);

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        MatchEngine.Instance.LogUnitDeath(identity);
        Destroy(gameObject);
    }
}
