
using UnityEngine;

public class DoubleBuffs : Buff
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Apply();
    }

    public override void Apply()
    {
        if (GetComponent<TurretController>())
        {
            Buff[] buffs = GetComponents<Buff>();
            foreach (Buff buff in buffs)
            {
                if(buff != this)
                {
                    Debug.Log("Previous Value: " + buff.buffAmount);
                    buff.buffAmount *= 2;
                    buff.Apply();
                    Debug.Log("Post Value: " + buff.buffAmount);
                }

            }
            if (GetComponent<BurnEffect>())
            {
                GetComponent<BurnEffect>().damage *= 2;
            }
        }
    }
}
