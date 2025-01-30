using Unity.Netcode;
using UnityEngine;

public class AttackModifierEffect : NetworkBehaviour
{
    public float duration;
    [HideInInspector] public float durationTimer;

    public virtual void EffectUpdate()
    {
        if (GetComponent<EnemyHealth>())
        {
            durationTimer += Time.deltaTime;
            if(durationTimer >= duration)
            {
                Destroy(this);
            }
        }
    }
}
