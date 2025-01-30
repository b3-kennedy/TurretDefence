using UnityEngine;

public class BurnDamageBuff : Buff
{
    public float damage;
    public float interval;
    public float duration;


    private void Start()
    {
        Apply();
    }

    public override void Apply()
    {
        if (GetComponent<TurretController>())
        {
            var playerBurn = gameObject.AddComponent<BurnEffect>();
            playerBurn.damage = damage;
            
            playerBurn.interval = interval;
            playerBurn.duration = duration;
        }
        else
        {
            buffAmount = damage;
        }
    }
}
