using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections;
using TMPro;


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

    public List<Buff> buffs = new List<Buff>();

    public GameObject rerollButton;

    public TextMeshProUGUI rerollText;

    List<GameObject> spawnedCards = new List<GameObject>();

    public int maxNumberOfRerolls = 1;
    int numberOfRerolls;

    public float escalatingValue;
    float rarityChange;

    int legendaryCount = 0;


    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        numberOfRerolls = maxNumberOfRerolls;
        
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
            var debuff = debuffs[index].debuff.GetType();
            if (EnemySpawnManager.Instance.localPlayer.GetComponent(debuff))
            {
                var comp = EnemySpawnManager.Instance.localPlayer.GetComponent(debuff);
                if(comp is Debuff debuffType)
                {
                    if (debuffAmount < 1)
                    {
                        debuffType.debuffAmount *= (1f - debuffAmount);
                        debuffType.Apply();
                        debuffType.count++;
                    }
                    else
                    {
                        EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().maxAmmoCount -= (int)debuffAmount;
                        EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().ammoCount = EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().maxAmmoCount;
                        debuffType.debuffAmount += debuffAmount;
                        debuffType.count++;
                    }
                }
            }
            else
            {
                var debuffComp = EnemySpawnManager.Instance.localPlayer.AddComponent(debuffs[index].debuff.GetType());

                if (debuffComp is Debuff debuffType)
                {
                    debuffType.debuffAmount = debuffAmount;
                    debuffType.count++;
                    debuffType.debuffName = debuffs[index].debuff.debuffName;
                }
            }

        }

        //var buff = buffScript.GetType();
        //if (EnemySpawnManager.Instance.localPlayer.GetComponent(buff))
        //{
        //    var comp = EnemySpawnManager.Instance.localPlayer.GetComponent(buff);
        //    if (comp is Buff buffType)
        //    {
        //        if (buffScript.buffAmount < 0)
        //        {
        //            buffType.buffAmount *= (1f - buffScript.buffAmount);
        //            buffType.Apply();
        //        }
        //        else
        //        {
        //            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().maxAmmoCount += (int)buffScript.buffAmount;
        //            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().ammoCount = EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().maxAmmoCount;
        //            buffType.buffAmount += buffScript.buffAmount;
        //        }


        //    }
        //}
        //else
        //{
        //    var buffComp = EnemySpawnManager.Instance.localPlayer.AddComponent(buffScript.GetType());
        //    if (buffComp is Buff buffType)
        //    {
        //        buffType.buffAmount = buffScript.buffAmount;
        //    }
        //}

    }

    public void ShowInterface() 
    {
        legendaryCount = 0;
        AudioManager.Instance.DeactivateMusicLowPass();
        upgradeUi.SetActive(true);
        rerollText.text = "Number of Rerolls: " + numberOfRerolls;
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
                    Card card = Instantiate(rarityAndSpawnChances[j].cards[cardIndex], cardParent).GetComponent<Card>();
                    spawnedCards.Add(card.gameObject);

                    if(card.rarity == RarityAndSpawnChance.Rarity.LEGENDARY)
                    {
                        legendaryCount++;
                    }

                    if(card.rarity == RarityAndSpawnChance.Rarity.DEMONIC)
                    {
                        AudioManager.Instance.MusicToZeroServerRpc();
                        AudioManager.Instance.PlayDemonServerRpc();
                    }
                    else if(card.rarity == RarityAndSpawnChance.Rarity.DIVINE)
                    {
                        AudioManager.Instance.MusicToZeroServerRpc();
                        AudioManager.Instance.PlayAngelServerRpc();
                    }

                    break;
                }
            }
        }


    }

    public void Reroll()
    {
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            Destroy(spawnedCards[i]);
        }
        spawnedCards.Clear();
        ShowInterface();
        numberOfRerolls--;
        rerollText.text = "Number of Rerolls: " + numberOfRerolls.ToString();
        if(numberOfRerolls <= 0)
        {
            rerollButton.GetComponent<Button>().interactable = false;
        }
    }

    public GameObject GetInterface()
    {
        return upgradeUi;
    }

    public void HideInterface()
    {
        Cursor.visible = true;
        upgradeUi.SetActive(false);
        numberOfRerolls = maxNumberOfRerolls;
        rerollButton.GetComponent <Button>().interactable = true;
        for (int i = 0;i < cardParent.childCount; i++)
        {
            Destroy(cardParent.GetChild(i).gameObject);
        }
        EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = true;
        if (legendaryCount == 0)
        {
            rarityAndSpawnChances[0].spawnChance -= escalatingValue;
            rarityAndSpawnChances[3].spawnChance += escalatingValue;
            rarityChange += escalatingValue;
        }
        else
        {
            rarityAndSpawnChances[0].spawnChance += rarityChange;
            rarityAndSpawnChances[3].spawnChance -= rarityChange;
            rarityChange = 0;
        }
    }

    private void Update()
    {
        if (upgradeUi != null && upgradeUi.activeSelf)
        {
            if(EnemySpawnManager.Instance.localPlayer != null)
            {
                EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = false;
            }
            
        }
    }

}
