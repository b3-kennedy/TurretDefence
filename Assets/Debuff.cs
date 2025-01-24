using Unity.Netcode;
using UnityEngine;

public class Debuff : NetworkBehaviour
{
    public bool canRandomlyAssignValue = true;
    [HideInInspector] public ScriptableObject debuffValues;
    [HideInInspector] public RarityAndSpawnChance.Rarity rarity;
    public float debuffAmount;
    public string debuffName;

    public virtual void Setup() 
    {
        if (GetComponent<Card>())
        {
            GetComponent<Card>().UpdateText();
        }
    }
}
