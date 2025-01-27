
using UnityEngine;

public class IncreaseBuffsBuff : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<TurretController>())
        {

            GetComponent<TurretController>().rotationSpeed = 90f;
            GetComponent<TurretController>().maxAmmoCount = 10;
            GetComponent<TurretController>().fireRate = 1;
            GetComponent<TurretController>().damage = 50f;

            foreach (var buff in GetComponents<Buff>())
            {
                buff.buffAmount *= buffAmount;
                buff.Apply();
            }
        
        }
    }

}
