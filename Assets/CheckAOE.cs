using Unity.Netcode;
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
                ulong playerId = GetComponent<DealDamage>().player.GetComponent<NetworkObject>().NetworkObjectId;
                collider.GetComponent<EnemyHealth>().TakeDamageServerRpc(GetComponent<DealDamage>().damage / 2f, playerId);
            }
        }
    }
}
