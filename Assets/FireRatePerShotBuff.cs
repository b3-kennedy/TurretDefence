using UnityEngine;

public class FireRatePerShotBuff : Buff
{

    float increasedBy;
    TurretController turretController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        if (GetComponent<TurretController>())
        {
            turretController = GetComponent<TurretController>();
            turretController.shot.AddListener(IncreaseFireRate);
            turretController.stoppedShooting.AddListener(DecreaseToNormalFireRate);
        }
    }

    void IncreaseFireRate()
    {
        if (turretController)
        {
            if(turretController.fireRate > 0.1f)
            {
                turretController.fireRate -= buffAmount;
                increasedBy += buffAmount;
            }
            else
            {
                turretController.fireRate = 0.1f;
            }

        }
    }

    void DecreaseToNormalFireRate()
    {
        turretController.fireRate += increasedBy;
        increasedBy = 0;
    }

}
