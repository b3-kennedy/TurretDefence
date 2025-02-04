using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class EnemyHealth : NetworkBehaviour
{
    public float maxHealth = 100;
    public NetworkVariable<float> health;
    public List<GameObject> bloodSplatters = new List<GameObject>();
    public List<GameObject> deathSplatters = new List<GameObject>();


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
    public void TakeDamageServerRpc(float dmg, ulong playerId, Vector3 hitDirection)
    {
        health.Value -= dmg;
        if (health.Value > 0)
        {
            int randomNum = Random.Range(0, bloodSplatters.Count);
            GameObject splatter = Instantiate(bloodSplatters[randomNum], transform.position, Quaternion.identity);
            SpriteRenderer mainRenderer = GetComponent<SpriteRenderer>();
            SpriteRenderer splatterRenderer = splatter.GetComponentInChildren<SpriteRenderer>();
            Color originalColor = mainRenderer.color;
            Color darkerColor = originalColor * 0.5f;
            splatterRenderer.color = darkerColor;
            splatter.GetComponent<NetworkObject>().Spawn();
        }
        else if(health.Value <= 0)
        {
            DeactivateGameObjectClientRpc(gameObject.GetComponent<NetworkObject>().NetworkObjectId, playerId);
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out var player))
            {
                player.GetComponent<TurretController>().killCount.Value++;
            }
            //spawn a blood splatter rotation based on the angle from projectile that hit this gameobject
            int randomNum = Random.Range(0, bloodSplatters.Count);
            float angle = Mathf.Atan2(hitDirection.y, hitDirection.x) * Mathf.Rad2Deg;
            Quaternion bloodRotation = Quaternion.Euler(0, 0, angle+180f);
            GameObject splatter = Instantiate(deathSplatters[randomNum], transform.position, bloodRotation);
            SpriteRenderer mainRenderer = GetComponent<SpriteRenderer>();
            SpriteRenderer splatterRenderer = splatter.GetComponentInChildren<SpriteRenderer>();
            Color originalColor = mainRenderer.color;
            Color darkerColor = originalColor * 0.5f;
            splatterRenderer.color = darkerColor;
            splatter.GetComponent<NetworkObject>().Spawn();
        }
    }

    [ClientRpc]
    void DeactivateGameObjectClientRpc(ulong id, ulong playerId)
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
    public void ApplyEffectServerRpc(ulong objectId, float dmg, float duration, float interval, ulong playerId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var enemyObj))
        {

            if (!enemyObj.GetComponent<BurnEffect>())
            {
                var burn = enemyObj.gameObject.AddComponent<BurnEffect>();
                burn.duration = duration;
                burn.interval = interval;
                burn.damage = dmg;
                burn.playerId = playerId;
            }
            else
            {
                enemyObj.GetComponent<BurnEffect>().duration = duration;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ApplySlowServerRpc(ulong objectId, float duration, float slowAmount)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var enemyObj))
        {

            if (!enemyObj.GetComponent<SlowEffect>())
            {
                var slow = enemyObj.gameObject.AddComponent<SlowEffect>();
                slow.duration = duration;
                slow.slowAmount = slowAmount;
            }
            else
            {
                enemyObj.GetComponent<SlowEffect>().duration = duration;
            }
        }
    }
}
