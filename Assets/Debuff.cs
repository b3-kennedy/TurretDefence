using Unity.Netcode;
using UnityEngine;

public class Debuff : NetworkBehaviour
{
    public bool canRandomlyAssignValue = true;
    public bool applyToSelf;
    public bool canBeRandomlyChosen = true;
    public bool isPercentage = true;
    public bool hasValue = true;
    [HideInInspector] public ScriptableObject debuffValues;
    [HideInInspector] public RarityAndSpawnChance.Rarity rarity;
    public float debuffAmount;
    public string debuffName;
    [HideInInspector] public int count;

    public virtual void Setup() 
    {
        if (GetComponent<Card>())
        {
            GetComponent<Card>().UpdateText();
        }
    }

    public virtual void Apply() { }
}
