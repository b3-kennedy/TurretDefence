using Unity.Netcode;
using UnityEngine;

public class Buff : NetworkBehaviour
{
    
    [HideInInspector] public RarityAndSpawnChance.Rarity rarity;
    public bool canRandomlyAssignValue = true;
    public bool isAttackEffect;
    public float buffAmount;
    public string buffName = "Fire Rate";
    [HideInInspector] public int count;

    public virtual void Apply()
    {
        
    }
}
