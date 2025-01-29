using UnityEngine;
using System.Collections.Generic;

public class BuffAndDebuffManager : MonoBehaviour
{

    public List<Buff> buffs = new List<Buff>();
    public List<Debuff> debuffs = new List<Debuff>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            buffs.Clear();
            debuffs.Clear();

            buffs.AddRange(GetComponents<Buff>());

            foreach (var buff in buffs) 
            {
                if(buff.buffAmount < 0)
                {
                    buffs.Remove(buff);
                }
            }

            debuffs.AddRange(GetComponents<Debuff>());
        }
    }
}
