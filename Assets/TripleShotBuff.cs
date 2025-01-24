using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class TripleShotBuff : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<TurretController>())
        {
            UpgradeManager.Instance.rarityAndSpawnChances[3].cards.Remove(gameObject);
            GetComponent<TurretController>().UpdateShootPointCountServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
