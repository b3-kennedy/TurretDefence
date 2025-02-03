using Unity.Netcode;
using UnityEngine;

public class BurnDamageBuff : Buff
{
    public float damage;
    public float interval;
    public float duration;


    private void Awake()
    {
        
    }

    private void Start()
    {
        Apply();
    }

    public override void Apply()
    {
        if (GetComponent<TurretController>())
        {
            if (!GetComponent<BurnEffect>())
            {
                
                var playerBurn = gameObject.AddComponent<BurnEffect>();
                playerBurn.damage = damage;

                playerBurn.interval = interval;
                playerBurn.duration = duration;

                Debug.Log(damage);
            }
            else
            {
                GetComponent<BurnEffect>().damage += damage;
            }

        }
        else
        {
            buffAmount = damage;
        }
    }
}


