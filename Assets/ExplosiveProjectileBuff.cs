using Unity.Netcode;
using UnityEngine;

public class ExplosiveProjectileBuff : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<TurretController>())
        {
            UpgradeManager.Instance.rarityAndSpawnChances[2].cards.Remove(gameObject);
            GetComponent<TurretController>().shootForce = 10f;
            GetComponent<TurretController>().ChangeProjectileServerRpc(GetComponent<TurretController>().GetComponent<NetworkObject>().NetworkObjectId, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
