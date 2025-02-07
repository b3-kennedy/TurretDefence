using UnityEngine;
using Unity.Netcode;

public class RandomProjectilePerShotBuff : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Apply();
    }

    public override void Apply()
    {
        if (GetComponent<TurretController>())
        {
            GetComponent<TurretController>().shot.AddListener(RandomProjectile);
        }
    }

    void RandomProjectile()
    {
        int randomNum = Random.Range(0, GetComponent<TurretController>().projectiles.Count);
        GetComponent<TurretController>().ChangeProjectileServerRpc(GetComponent<TurretController>().GetComponent<NetworkObject>().NetworkObjectId, randomNum);
    }
}
