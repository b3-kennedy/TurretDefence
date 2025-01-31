using UnityEngine;

public class SlowEffect : AttackModifierEffect
{

    public float slowAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<EnemyController>())
        {
            GetComponent<EnemyController>().speed *= slowAmount;
        }
    }

    public override void OnDestroy()
    {
        if (GetComponent<EnemyController>())
        {
            GetComponent<EnemyController>().speed /= slowAmount;
        }
    }

    // Update is called once per frame
    void Update()
    {
        EffectUpdate();
    }

    public override void EffectUpdate()
    {
        base.EffectUpdate();
    }
}
