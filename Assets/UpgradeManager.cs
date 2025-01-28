using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.Rendering;

[System.Serializable]
public class RarityAndSpawnChance
{
    public enum Rarity {COMMON, RARE, EPIC, LEGENDARY, DIVINE, DEMONIC};
    public Rarity rarity;
    public float spawnChance;
    public List<GameObject> cards;

}

[System.Serializable]
public class DebuffAndName
{
    public Debuff debuff;
    public string debuffName;
    public ScriptableObject values;
    
}

public class UpgradeManager : NetworkBehaviour
{

    public List<RarityAndSpawnChance> rarityAndSpawnChances = new List<RarityAndSpawnChance>();

    public static UpgradeManager Instance;

    public GameObject upgradeUi;

    public Transform cardParent;

    public List<DebuffAndName> debuffs;


    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void ApplyDebuffServerRpc(ulong clientId, int index, float debuffAmount)
    {
       ApplyDebuffClientRpc(clientId, index, debuffAmount);
    }

    [ClientRpc]
    void ApplyDebuffClientRpc(ulong clientId, int index, float debuffAmount)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId)
        {
            var debuffComp = EnemySpawnManager.Instance.localPlayer.AddComponent(debuffs[index].debuff.GetType());

            if (debuffComp is Debuff debuff)
            {
                debuff.debuffAmount = debuffAmount;
            }
        }

    }

    public void ShowInterface() 
    {
        upgradeUi.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            float num = Random.Range(0, 100f);
            
            float cumulativeChance = 0;
            for (int j = 0; j < rarityAndSpawnChances.Count; j++) 
            {
                cumulativeChance += rarityAndSpawnChances[j].spawnChance;
                
                if (num < cumulativeChance)
                {
                    int cardIndex = Random.Range(0, rarityAndSpawnChances[j].cards.Count);
                    Instantiate(rarityAndSpawnChances[j].cards[cardIndex], cardParent);

                    break;
                }
            }
        }
    }

    public GameObject GetInterface()
    {
        return upgradeUi;
    }

    public void HideInterface()
    {
        upgradeUi.SetActive(false);
        for (int i = 0;i < cardParent.childCount; i++)
        {
            Destroy(cardParent.GetChild(i).gameObject);
        }
        EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = true;
    }

}
