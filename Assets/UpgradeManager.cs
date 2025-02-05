using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;


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
        AudioManager.Instance.DeactivateMusicLowPass();
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
        Cursor.visible = true;
        upgradeUi.SetActive(false);
        for (int i = 0;i < cardParent.childCount; i++)
        {
            Destroy(cardParent.GetChild(i).gameObject);
        }
        EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = true;
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
