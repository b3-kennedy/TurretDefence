using Unity.Netcode;
using UnityEngine;

public class RailgunBuff : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<TurretController>())
        {
            GetComponent<TurretController>().ChangeProjectileServerRpc(GetComponent<TurretController>().GetComponent<NetworkObject>().NetworkObjectId, 3);
            GetComponent<TurretController>().shootForce = 50f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
