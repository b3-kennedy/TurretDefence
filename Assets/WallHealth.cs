using Unity.Netcode;
using UnityEngine;

public class WallHealth : NetworkBehaviour
{
    public NetworkVariable<float> wallHealth;

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float dmg)
    {
        if (!IsServer) return;

        if(wallHealth.Value > 0)
        {
            wallHealth.Value -= dmg;
        }
        else
        {
            EnemySpawnManager.Instance.GameOverServerRpc();
        }
    }
}
