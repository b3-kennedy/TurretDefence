using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Card : NetworkBehaviour
{
    public RarityAndSpawnChance.Rarity rarity;

    public string titleString;

    [HideInInspector] public TextMeshProUGUI title;
    [HideInInspector] public TextMeshProUGUI buff;
    [HideInInspector] public TextMeshProUGUI debuffText;

    Buff buffScript;
    public Debuff debuffScript;

    float debuffAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buffScript = GetComponent<Buff>();

        title = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        buff = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        debuffText = transform.GetChild(5).GetComponent<TextMeshProUGUI>();

        title.text = buffScript.buffName;

        
        buffScript.rarity = rarity;

        if(rarity != RarityAndSpawnChance.Rarity.DIVINE)
        {
            int randomNum = Random.Range(0, UpgradeManager.Instance.debuffs.Count);

            var debuffAndName = UpgradeManager.Instance.debuffs[randomNum];
            var debuffToAdd = debuffAndName.debuff;
            var debuffValues = debuffAndName.values;

            var addedDebuff = gameObject.AddComponent(debuffToAdd.GetType());

            if (addedDebuff is Debuff debuff)
            {
                debuff.rarity = rarity;
                debuffScript = debuff;
                debuffScript.debuffName = debuffAndName.debuffName;
                debuffScript.debuffValues = debuffValues;
                debuffScript.Setup();
                debuffAmount = debuffScript.debuffAmount;

            }





            
        }

        GetComponent<Button>().onClick.AddListener(ApplyEffects);
    }

    public void UpdateText()
    {
        if(buffScript.buffAmount < 1)
        {
            buff.text = "+ " + (buffScript.buffAmount * 100).ToString("F1") + "% " + buffScript.buffName;
        }
        else
        {
            buff.text = "+ " + (buffScript.buffAmount).ToString("F1") + " " + buffScript.buffName;
        }

        if(debuffScript.debuffAmount < 1)
        {
            debuffText.text = "- " + (debuffScript.debuffAmount * 100).ToString("F1") + "% " + debuffScript.debuffName;
        }
        else
        {
            debuffText.text = "- " + (debuffScript.debuffAmount).ToString("F1") + " " + debuffScript.debuffName;
        }
        
    }

    public void ApplyEffects()
    {

        if (EnemySpawnManager.Instance.localPlayer.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId) return;

        var buffComp = EnemySpawnManager.Instance.localPlayer.AddComponent(buffScript.GetType());

        if(buffComp is Buff buff) 
        {
            buff.buffAmount = buffScript.buffAmount;
        }


        if(debuffScript != null)
        {
            int index = 0;
            foreach (var debuff in UpgradeManager.Instance.debuffs)
            {
                if (debuff.debuff.GetType() == debuffScript.GetType())
                {
                    UpgradeManager.Instance.ApplyDebuffServerRpc(NetworkManager.Singleton.LocalClientId, index, debuffAmount);
                }
                index++;
            }
        }


        

        UpgradeManager.Instance.HideInterface();

    }






}
