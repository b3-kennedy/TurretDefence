using Unity.Netcode;
using UnityEngine;

public class EnemyProjectile : NetworkBehaviour
{

    [HideInInspector] public float damage;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!IsServer) return;

        if (other.collider.CompareTag("Wall"))
        {
            other.gameObject.GetComponent<WallHealth>().TakeDamageServerRpc(damage);
            Destroy(gameObject);
        }
    }
}
