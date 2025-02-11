using UnityEngine;

public class DamageBuff : Buff
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
            GetComponent<TurretController>().damage *= (1f + buffAmount);
        }
        else
        {
            if(values != null && canRandomlyAssignValue)
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
            }


            if (GetComponent<Card>())
            {
                GetComponent<Card>().UpdateText();
            }
        }
    }

}
