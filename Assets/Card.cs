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

        int numDebuffs = Random.Range(0, debuffCount); // Add 1-2 debuffs
        for (int i = 0; i < numDebuffs; i++)
        {
            int randomNum = Random.Range(0, UpgradeManager.Instance.debuffs.Count);

            var debuffAndName = UpgradeManager.Instance.debuffs[randomNum];
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
            if (buffScript.buffAmount < 1 && buffScript.buffAmount >= 0)
            {
                buffText += "+ " + (buffScript.buffAmount * 100).ToString("F1") + "% " + buffScript.buffName + "\n";
            }
            else if(buffScript.buffAmount >= 1)
            {
                buffText += "+ " + (buffScript.buffAmount).ToString("F1") + " " + buffScript.buffName + "\n";
            }
            else if(buffScript.buffAmount < 0)
            {
                buffText += "- " + (buffScript.buffAmount*-1).ToString("F1") + " " + buffScript.buffName + "\n";
            }
            else if(buffScript.buffAmount == 0)
            {
                buffText += "+ " + buffScript.buffName + "\n";
            }
        }
        buff.text = buffText.TrimEnd();

        // Update Debuff Text
        string deText = "";
        foreach (var debuffScript in debuffScripts)
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

    public void ApplyEffects()
    {
        if (EnemySpawnManager.Instance.localPlayer.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId) return;

        // Apply all Buffs
        foreach (var buffScript in buffScripts)
        {
            var buffComp = EnemySpawnManager.Instance.localPlayer.AddComponent(buffScript.GetType());
            if (buffComp is Buff buff)
            {
                buff.buffAmount = buffScript.buffAmount;
            }
        }

        // Apply all Debuffs
        foreach (var debuffScript in debuffScripts)
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

        UpgradeManager.Instance.HideInterface();
    }
}








