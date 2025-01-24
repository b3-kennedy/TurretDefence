using Unity.Netcode;
using UnityEngine;

public class Buff : NetworkBehaviour
{
    
    [HideInInspector] public RarityAndSpawnChance.Rarity rarity;
    public bool canRandomlyAssignValue = true;
    public float buffAmount;
    public string buffName = "Fire Rate";

    public virtual void Apply()
    {

    }
}
