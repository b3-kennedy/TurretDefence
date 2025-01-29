using Unity.Netcode;
using UnityEngine;

public class GasBulletBuff : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<TurretController>())
        {
            GetComponent<TurretController>().ChangeProjectileServerRpc(GetComponent<TurretController>().GetComponent<NetworkObject>().NetworkObjectId, 4);
        }
    }
}
