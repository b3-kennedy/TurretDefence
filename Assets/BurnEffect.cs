using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class BurnEffect : AttackModifierEffect
{
    public float damage;
    public float interval;
    float timer;


    private void Update()
    {
        EffectUpdate();
    }

    public override void EffectUpdate()
    {

        base.EffectUpdate();

        if (GetComponent<EnemyHealth>())
        {
            Debug.Log("burning");

            timer += Time.deltaTime;
            if(timer >= interval)
            {
                GetComponent<EnemyHealth>().TakeDamageServerRpc(damage);
                timer = 0;
            }
            
        }
    }
}
