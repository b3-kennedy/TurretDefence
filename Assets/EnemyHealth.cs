using Unity.Netcode;
using UnityEngine;

public class EnemyHealth : NetworkBehaviour
{
    public float maxHealth = 100;
    public NetworkVariable<float> health;


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = maxHealth;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float dmg)
    {
        health.Value -= dmg;
        if(health.Value <= 0)
        {
            DeactivateGameObjectClientRpc(gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    [ClientRpc]
    void DeactivateGameObjectClientRpc(ulong id)
    {

        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(id, out var enemyObj))
        {
            enemyObj.gameObject.SetActive(false);
            if (IsServer)
            {
                EnemySpawnManager.Instance.IsWaveOver();
            }
        }

    }
}
