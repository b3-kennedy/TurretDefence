using UnityEngine;
using static RarityAndSpawnChance;

public class DecreaseDropRateDebuff : Debuff
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
            float legendaryRarity = UpgradeManager.Instance.rarityAndSpawnChances[3].spawnChance / 2;
            float divineRarity = UpgradeManager.Instance.rarityAndSpawnChances[4].spawnChance / 2;
            float demonicRarity = UpgradeManager.Instance.rarityAndSpawnChances[5].spawnChance / 2;

            float combined = legendaryRarity + divineRarity + demonicRarity;

            UpgradeManager.Instance.rarityAndSpawnChances[0].spawnChance += combined;

            UpgradeManager.Instance.rarityAndSpawnChances[3].spawnChance = legendaryRarity;
            UpgradeManager.Instance.rarityAndSpawnChances[4].spawnChance = divineRarity;
            UpgradeManager.Instance.rarityAndSpawnChances[5].spawnChance = demonicRarity;
        }
    }

    public override void Setup()
    {
        base.Setup();
    }

    public override void OnDestroy()
    {
        if (GetComponent<TurretController>())
        {
            float legendaryRarity = UpgradeManager.Instance.rarityAndSpawnChances[3].spawnChance * 2;
            float divineRarity = UpgradeManager.Instance.rarityAndSpawnChances[4].spawnChance * 2;
            float demonicRarity = UpgradeManager.Instance.rarityAndSpawnChances[5].spawnChance * 2;

            float combined = legendaryRarity + divineRarity + demonicRarity;

            UpgradeManager.Instance.rarityAndSpawnChances[0].spawnChance -= combined;

            UpgradeManager.Instance.rarityAndSpawnChances[3].spawnChance = legendaryRarity;
            UpgradeManager.Instance.rarityAndSpawnChances[4].spawnChance = divineRarity;
            UpgradeManager.Instance.rarityAndSpawnChances[5].spawnChance = demonicRarity;
        }
    }
}
