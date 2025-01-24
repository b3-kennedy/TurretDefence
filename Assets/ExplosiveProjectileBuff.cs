using Unity.Netcode;
using UnityEngine;

public class ExplosiveProjectileBuff : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<TurretController>())
        {
            GetComponent<TurretController>().ChangeProjectileServerRpc(GetComponent<TurretController>().GetComponent<NetworkObject>().NetworkObjectId, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
