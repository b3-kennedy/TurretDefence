using UnityEngine;
using static RarityAndSpawnChance;

public class FireRateDebuff : Debuff
{

    public FloatRarityValues values;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Apply();
    }

    public override void Apply()
    {
        if (GetComponent<TurretController>())
        {
            GetComponent<TurretController>().fireRate *= (1f + debuffAmount);
        }
    }

    public override void Setup()
    {
        if (debuffValues is FloatRarityValues val)
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
            GetComponent<TurretController>().reloadTime *= (1f - debuffAmount);
        }
    }
}
