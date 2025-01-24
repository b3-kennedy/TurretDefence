using System;
using UnityEngine;

public class CleanseAllDebuffs : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<TurretController>())
        {
            Debuff[] components = gameObject.GetComponents<Debuff>();
            for (int i = 0; i < components.Length; i++)
            {
                Destroy(components[i]);
            }
        }
    }

}
