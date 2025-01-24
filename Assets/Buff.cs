using Unity.Netcode;
using UnityEngine;

public class Buff : NetworkBehaviour
{
    
    [HideInInspector] public RarityAndSpawnChance.Rarity rarity;
    public float buffAmount;
    public string buffName = "Fire Rate";
}
