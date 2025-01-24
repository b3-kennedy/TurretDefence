using Unity.Netcode;
using UnityEngine;

public class ReloadAllyBuff : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
        if (GetComponent<TurretController>())
        {
            GetComponent<TurretController>().reloaded.AddListener(Reload);
        }
        
    }

    void Reload()
    {
        ulong otherId = 0;
        if(GetComponent<TurretController>().gameObject.GetComponent<NetworkObject>().OwnerClientId == 0)
        {
            otherId = 1;
        }
        else
        {
            otherId = 0;
        }

        GetComponent<TurretController>().ReloadedByAllyServerRpc(otherId);
    }

    public override void Apply()
    {
        if (GetComponent<TurretController>())
        {
            GetComponent<TurretController>().rotationSpeed *= (1f + buffAmount);
        }
        else
        {

            if (GetComponent<Card>())
            {
                GetComponent<Card>().UpdateText();
            }
        }
    }
}
