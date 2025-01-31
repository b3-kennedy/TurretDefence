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

    public void UpdateScale()
    {
        float value = maxHealth / 100f;
        transform.localScale = new Vector3(value, value, 1);
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

    [ServerRpc(RequireOwnership = false)]
    public void ApplyEffectServerRpc(ulong objectId, float dmg, float duration, float interval)
    {
        Debug.Log("here");
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var enemyObj))
        {

            if (!enemyObj.GetComponent<BurnEffect>())
            {
                var burn = enemyObj.gameObject.AddComponent<BurnEffect>();
                burn.duration = duration;
                burn.interval = interval;
                burn.damage = dmg;
            }
            else
            {
                enemyObj.GetComponent<BurnEffect>().duration = duration;
            }
        }
    }
}
