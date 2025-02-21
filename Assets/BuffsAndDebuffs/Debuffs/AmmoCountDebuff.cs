using TMPro;
using UnityEngine;

public class AmmoCountDebuff : Debuff
{
    public IntRarityValues values;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Apply();
    }

    public override void Apply()
    {
        if (GetComponent<TurretController>())
        {
            GetComponent<TurretController>().maxAmmoCount -= (int)debuffAmount;
            GetComponent<TurretController>().UpdateAmmotext();
        }
    }

    public override void Setup()
    {
        if (debuffValues is IntRarityValues val)
        {
            values = val;
        }

        if (canRandomlyAssignValue)
        {
            switch (rarity)
            {
                case RarityAndSpawnChance.Rarity.COMMON:
                    debuffAmount = Random.Range(values.minCommonValue, values.maxCommonValue);
                    break;
                case RarityAndSpawnChance.Rarity.RARE:
                    debuffAmount = Random.Range(values.minRareValue, values.maxRareValue);
                    break;
                case RarityAndSpawnChance.Rarity.EPIC:
                    debuffAmount = Random.Range(values.minEpicValue, values.maxEpicValue);
                    break;
                case RarityAndSpawnChance.Rarity.LEGENDARY:
                    debuffAmount = Random.Range(values.minLegendaryValue, values.maxLegendaryValue);
                    break;
            }
        }


        base.Setup();
    }

    public override void OnDestroy()
    {
        if (GetComponent<TurretController>())
        {
            GetComponent<TurretController>().maxAmmoCount += (int)debuffAmount;
        }
    }


}


