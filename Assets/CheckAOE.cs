using UnityEngine;

public class CheckAOE : MonoBehaviour
{
    public float radius;

    public void DealDamageAOE()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<EnemyHealth>())
            {
                collider.GetComponent<EnemyHealth>().TakeDamageServerRpc(GetComponent<DealDamage>().damage / 2f);
            }
        }
    }
}
