using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DealDamage : NetworkBehaviour
{



    public float damage;

    private void Start()
    {

        //StartCoroutine(DestroyAfterTime(5f));

    }

    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyOnServerRpc(GetComponent<NetworkObject>().NetworkObjectId);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.GetComponent<EnemyHealth>())
        {
            var id = GetComponent<NetworkObject>().NetworkObjectId;
            other.transform.GetComponent<EnemyHealth>().TakeDamageServerRpc(damage);
            gameObject.SetActive(false);
            

            

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
