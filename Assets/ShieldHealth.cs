using Unity.Netcode;
using UnityEngine;

public class ShieldHealth : NetworkBehaviour
{

    public NetworkVariable<float> health;

    private void Update()
    {
        if (!IsServer) return;

        transform.localPosition = new Vector2(-1.38f, 0); 
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        if(health.Value > 0)
        {
            health.Value -= damage;
        }
        
        if(health.Value <= 0)
        {
            Destroy(gameObject);
        }
    }

}
