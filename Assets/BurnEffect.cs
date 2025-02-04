
using Unity.Netcode;
using UnityEngine;

public class BurnEffect : AttackModifierEffect
{
    public float damage;
    public float interval;
    float timer;
    [HideInInspector] public ulong playerId;
    GameObject spawnedFire;


    private void Start()
    {
        if (GetComponent<EnemyHealth>())
        {
            EnemySpawnManager.Instance.CreateFireServerRpc(GetComponent<NetworkObject>().NetworkObjectId, duration);
        }
    }

    public override void OnDestroy()
    {
        if(spawnedFire != null)
        {
            Destroy(spawnedFire);
        }
    }

    private void Update()
    {
        EffectUpdate();
    }

    public override void EffectUpdate()
    {

        base.EffectUpdate();

        if (GetComponent<EnemyHealth>())
        {

            timer += Time.deltaTime;
            if(timer >= interval)
            {
                GetComponent<EnemyHealth>().TakeDamageServerRpc(damage, playerId, Vector3.zero);
                timer = 0;
            }
            
        }
    }
}
