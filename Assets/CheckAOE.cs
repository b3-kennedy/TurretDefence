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
                Vector3 dir = (transform.position - collider.transform.position).normalized;
                collider.GetComponent<EnemyHealth>().TakeDamageServerRpc(GetComponent<DealDamage>().damage / 2f, playerId, dir);
            }
        }

        EnemySpawnManager.Instance.CreateExplosionServerRpc(transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, radius);
    }

}
