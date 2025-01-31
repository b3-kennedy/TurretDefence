using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DealDamage : NetworkBehaviour
{


    [HideInInspector] public TurretController player;
    public float damage;

    private void Start()
    {

        //StartCoroutine(DestroyAfterTime(5f));
        if (GetComponent<NetworkObject>().IsSpawned)
        {
            if (IsServer)
            {
                Destroy(gameObject, 5f);
            }
        }
        else
        {
            Destroy(gameObject, 5f);
        }

    }



    void ApplyEffect(GameObject other)
    {
        Debug.Log(player);

        if (player.GetComponent<BurnEffect>())
        {
            
            var playerBurn = player.GetComponent<BurnEffect>();
            other.GetComponent<EnemyHealth>().ApplyEffectServerRpc(other.GetComponent<NetworkObject>().NetworkObjectId, playerBurn.damage,
                playerBurn.duration, playerBurn.interval, player.GetComponent<NetworkObject>().NetworkObjectId);

        }
        else if (player.GetComponent<SlowEffect>())
        {
            var playerSlow = player.GetComponent<SlowEffect>();
            other.GetComponent<EnemyHealth>().ApplySlowServerRpc(other.GetComponent<NetworkObject>().NetworkObjectId, 
                playerSlow.duration, playerSlow.slowAmount);

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.GetComponent<ShieldHealth>())
        {
            other.transform.GetComponent<ShieldHealth>().TakeDamageServerRpc(damage);
            ApplyEffect(other.gameObject);

            if (GetComponent<NetworkObject>().IsSpawned)
            {
                if (IsServer)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (other.transform.GetComponent<EnemyHealth>())
        {
            if (GetComponent<CheckAOE>())
            {
                GetComponent<CheckAOE>().DealDamageAOE();
            }
            var id = GetComponent<NetworkObject>().NetworkObjectId;
            other.transform.GetComponent<EnemyHealth>().TakeDamageServerRpc(damage, player.GetComponent<NetworkObject>().NetworkObjectId);
            ApplyEffect(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.GetComponent<ShieldHealth>())
        {
            other.transform.GetComponent<ShieldHealth>().TakeDamageServerRpc(damage);
            ApplyEffect(other.gameObject);

            if (GetComponent<NetworkObject>().IsSpawned)
            {
                if (IsServer)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (other.transform.GetComponent<EnemyHealth>())
        {
            if (GetComponent<CheckAOE>())
            {
                GetComponent<CheckAOE>().DealDamageAOE();
            }
            var id = GetComponent<NetworkObject>().NetworkObjectId;
            other.transform.GetComponent<EnemyHealth>().TakeDamageServerRpc(damage, player.GetComponent<NetworkObject>().NetworkObjectId);
            ApplyEffect(other.gameObject);

            if (GetComponent<NetworkObject>().IsSpawned)
            {
                if (IsServer)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }


            
            
            

            

        }
    }


    [ServerRpc(RequireOwnership = false)]
    void DestroyOnServerRpc(ulong objId)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objId, out var bullet))
        {
            bullet.Despawn();
        }
    }
}
