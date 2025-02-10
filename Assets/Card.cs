using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class Card : NetworkBehaviour
{
    public RarityAndSpawnChance.Rarity rarity;

    public int debuffCount = 1;

    public string titleString;

    [HideInInspector] public TextMeshProUGUI title;
    [HideInInspector] public TextMeshProUGUI buff;
    [HideInInspector] public TextMeshProUGUI debuffText;

    List<Buff> buffScripts = new List<Buff>();
    List<Debuff> debuffScripts = new List<Debuff>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        title = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        buff = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        debuffText = transform.GetChild(5).GetComponent<TextMeshProUGUI>();

        

        // Generate Buffs
        buffScripts.AddRange(GetComponents<Buff>());
        foreach (Buff buff in buffScripts)
        {
            buff.rarity = rarity;
        }

        debuffScripts.AddRange(GetComponents<Debuff>());
        foreach (Debuff debuff in debuffScripts)
        {
            debuff.rarity = rarity;
        }


         title.text = buffScripts[0].buffName;

        // Generate Debuffs


        if(debuffCount > 0)
        {
            int numDebuffs = Random.Range(1, debuffCount + 1);
            int addedDebuffCount = 0;

            while (addedDebuffCount < numDebuffs && UpgradeManager.Instance.debuffs.Count > 0)
            {
                int randomNum = Random.Range(0, UpgradeManager.Instance.debuffs.Count);
                var debuffAndName = UpgradeManager.Instance.debuffs[randomNum];

                if (debuffAndName.debuff.canBeRandomlyChosen)
                {
                    var debuffToAdd = debuffAndName.debuff;
                    var debuffValues = debuffAndName.values;

                    var addedDebuff = gameObject.AddComponent(debuffToAdd.GetType());

                    if (addedDebuff is Debuff debuff)
                    {
                        debuff.rarity = rarity;
                        debuff.debuffName = debuffAndName.debuffName;
                        debuff.debuffValues = debuffValues;
                        debuff.Setup();
                        debuffScripts.Add(debuff);
                        addedDebuffCount++; // Only count valid debuffs
                    }
                }
            }
        }



        GetComponent<Button>().onClick.AddListener(ApplyEffects);
        UpdateText();
    }

    public void UpdateText()
    {
        // Update Buff Text
        string buffText = "";

        foreach (var buffScript in buffScripts)
        {
            if (!buffScript.isAttackEffect)
            {
                if (buffScript.buffAmount < 1 && buffScript.buffAmount >= 0)
                {
                    buffText += "+ " + (buffScript.buffAmount * 100).ToString("F1") + "% " + buffScript.buffName + "\n";
                }
                else if (buffScript.buffAmount >= 1)
                {
                    buffText += "+ " + (buffScript.buffAmount).ToString("F1") + " " + buffScript.buffName + "\n";
                }
                else if (buffScript.buffAmount < 0)
                {
                    buffText += "- " + ((buffScript.buffAmount * -1) * 100).ToString("F1") + " " + buffScript.buffName + "\n";
                }
                else if (buffScript.buffAmount == 0)
                {
                    buffText += "+ " + buffScript.buffName + "\n";
                }
            }
            else
            {
                if(buffScript.buffAmount < 1)
                {
                    buffText += "Apply " + (buffScript.buffAmount*100).ToString("F1") + "% " + buffScript.buffName + " On Hit For 5 Seconds";
                }
                else
                {
                    buffText += "Apply " + buffScript.buffAmount.ToString("F1") + " " + buffScript.buffName + " On Hit For 5 Seconds";
                }
                
            }

        }

        //debuffs applied to self appear in top section of card
        foreach (var debuff in debuffScripts)
        {
            if (debuff.applyToSelf)
            {
                if (debuff.debuffAmount < 1 && debuff.debuffAmount >= 0)
                {
                    buffText += "- " + (debuff.debuffAmount * 100).ToString("F1") + "% " + debuff.debuffName + "\n";
                }
                else if (debuff.debuffAmount >= 1)
                {
                    buffText += "- " + (debuff.debuffAmount).ToString("F1") + " " + debuff.debuffName + "\n";
                }
                else if (debuff.debuffAmount < 0)
                {
                    buffText += "- " + ((debuff.debuffAmount * -1) * 100).ToString("F1") + " " + debuff.debuffName + "\n";
                }
                else if (debuff.debuffAmount == 0)
                {
                    buffText += "- " + debuff.debuffName + "\n";
                }
            }
        }

        buff.text = buffText.TrimEnd();

        // Update Debuff Text
        string deText = "";
        foreach (var debuffScript in debuffScripts)
        {
            if (!debuffScript.applyToSelf)
            {
                if (debuffScript.debuffAmount < 1)
                {
                    deText += "- " + (debuffScript.debuffAmount * 100).ToString("F1") + "% " + debuffScript.debuffName + "\n";
                }
                else
                {
                    deText += "- " + (debuffScript.debuffAmount).ToString("F1") + " " + debuffScript.debuffName + "\n";
                }
                debuffText.text = deText.TrimEnd();
            }


        }
        
    }

    public void ApplyEffects()
    {
        if (EnemySpawnManager.Instance.localPlayer.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId) return;

        // Apply all Buffs
        foreach (var buffScript in buffScripts)
        {
            
            var buff = buffScript.GetType();
            if (EnemySpawnManager.Instance.localPlayer.GetComponent(buff))
            {
                var comp = EnemySpawnManager.Instance.localPlayer.GetComponent(buff);
                if(comp is Buff buffType)
                {
                    if(buffScript.buffAmount < 1)
                    {
                        buffType.buffAmount *= (1f - buffScript.buffAmount);
                        buffType.count++;

                        buffType.Apply();
                    }
                    else
                    {
                        if (buffType is BurnDamageBuff burn)
                        {
                            EnemySpawnManager.Instance.localPlayer.GetComponent<BurnEffect>().damage += burn.damage;
                        }
                        else if(buffType is AmmoCountBuff)
                        {
                            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().maxAmmoCount += (int)buffScript.buffAmount;
                            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().ammoCount = EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().maxAmmoCount;
                        }
                        else if(buffType is FirstShotDamageBuff)
                        {
                            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().firstShotMultiplier += buffScript.buffAmount;
                        }
                        else if(buffType is BulletsPerShotBuff)
                        {
                            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().bulletsPerShot += (int)buffScript.buffAmount;
                        }
                        else if(buffType is RerollCountBuff)
                        {
                            UpgradeManager.Instance.maxNumberOfRerolls++;
                            UpgradeManager.Instance.numberOfRerolls = UpgradeManager.Instance.maxNumberOfRerolls;
                        }

                        buffType.buffAmount += buffScript.buffAmount;
                        buffType.count++;
                    }
                    
                    
                }
            }
            else
            {
                var buffComp = EnemySpawnManager.Instance.localPlayer.AddComponent(buffScript.GetType());
                if (buffComp is Buff buffType)
                {
                    buffType.buffAmount = buffScript.buffAmount;
                    buffType.count++;
                    buffType.buffName = buffScript.buffName;

                    if(buffType is BurnDamageBuff burn)
                    {
                        burn.damage = GetComponent<BurnDamageBuff>().damage;
                        burn.duration = GetComponent<BurnDamageBuff>().duration;
                        burn.interval = GetComponent<BurnDamageBuff>().interval;
                    }
                    else if(buffType is SlowEffectBuff slow)
                    {
                        slow.duration = GetComponent<SlowEffectBuff>().duration;
                        slow.slowAmount = GetComponent<SlowEffectBuff>().slowAmount;
                    }
                }



            }


        }

        // Apply all Debuffs
        foreach (var debuffScript in debuffScripts)
        {
            if (debuffScript.applyToSelf)
            {
                var debuff = debuffScript.GetType();
                if (EnemySpawnManager.Instance.localPlayer.GetComponent(debuff))
                {
                    var comp = EnemySpawnManager.Instance.localPlayer.GetComponent(debuff);
                    if (comp is Buff buffType)
                    {
                        if (debuffScript.debuffAmount < 1)
                        {
                            buffType.buffAmount *= (1f - debuffScript.debuffAmount);
                            buffType.count++;

                            buffType.Apply();
                        }
                        else
                        {
                            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().maxAmmoCount += (int)debuffScript.debuffAmount;
                            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().ammoCount = EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().maxAmmoCount;
                            buffType.buffAmount += debuffScript.debuffAmount;
                            buffType.count++;
                        }


                    }
                }
                else
                {
                    var debuffComp = EnemySpawnManager.Instance.localPlayer.AddComponent(debuffScript.GetType());
                    if (debuffComp is Debuff debuffType)
                    {
                        debuffType.debuffAmount = debuffScript.debuffAmount;
                        debuffType.count++;
                        debuffType.debuffName = debuffScript.debuffName;
                    }

                }
            }
            else
            {
                int index = 0;
                foreach (var debuff in UpgradeManager.Instance.debuffs)
                {
                    
                    if (debuff.debuff.GetType() == debuffScript.GetType())
                    {
                        UpgradeManager.Instance.ApplyDebuffServerRpc(NetworkManager.Singleton.LocalClientId, index, debuffScript.debuffAmount);
                    }
                    index++;
                }
            }


        }

        UpgradeManager.Instance.HideInterface();
    }
}








