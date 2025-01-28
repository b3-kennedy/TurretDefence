using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DealDamage : NetworkBehaviour
{



    public float damage;

    private void Start()
    {

        //StartCoroutine(DestroyAfterTime(5f));

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.GetComponent<EnemyHealth>())
        {
            if (GetComponent<CheckAOE>())
            {
                GetComponent<CheckAOE>().DealDamageAOE();
            }
            var id = GetComponent<NetworkObject>().NetworkObjectId;
            other.transform.GetComponent<EnemyHealth>().TakeDamageServerRpc(damage);

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
