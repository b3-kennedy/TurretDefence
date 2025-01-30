using Unity.Netcode;
using UnityEngine;

public class GasBulletBuff : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<TurretController>())
        {
            UpgradeManager.Instance.rarityAndSpawnChances[3].cards.Remove(gameObject);
            GetComponent<TurretController>().ChangeProjectileServerRpc(GetComponent<TurretController>().GetComponent<NetworkObject>().NetworkObjectId, 4);
            GetComponent<TurretController>().shootForce = 5f;
        }
    }
}
