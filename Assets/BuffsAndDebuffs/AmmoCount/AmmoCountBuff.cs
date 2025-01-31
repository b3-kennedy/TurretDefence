using UnityEngine;

public class AmmoCountBuff : Buff
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
            GetComponent<TurretController>().maxAmmoCount += (int)buffAmount;
            GetComponent<TurretController>().ammoCount = GetComponent<TurretController>().maxAmmoCount;
            GetComponent<TurretController>().UpdateAmmotext();
        }
        else
        {
            switch (rarity)
            {
                case RarityAndSpawnChance.Rarity.COMMON:
                    buffAmount = Random.Range(values.minCommonValue, values.maxCommonValue);
                    break;
                case RarityAndSpawnChance.Rarity.RARE:
                    buffAmount = Random.Range(values.minRareValue, values.maxRareValue);
                    break;
                case RarityAndSpawnChance.Rarity.EPIC:
                    buffAmount = Random.Range(values.minEpicValue, values.maxEpicValue);
                    break;
                case RarityAndSpawnChance.Rarity.LEGENDARY:
                    buffAmount = Random.Range(values.minLegendaryValue, values.maxLegendaryValue);
                    break;
            }

            if (GetComponent<Card>())
            {
                GetComponent<Card>().UpdateText();
            }
        }
    }
}

