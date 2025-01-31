using UnityEngine;

public class SlowEffectBuff : Buff
{
    public float slowAmount;
    public float duration;

    private void Awake()
    {
        Apply();
    }

    private void Start()
    {
        
    }

    public override void Apply()
    {
        if (GetComponent<TurretController>())
        {
            if (!GetComponent<SlowEffect>())
            {
                var slow = gameObject.AddComponent<SlowEffect>();
                slow.slowAmount = slowAmount;
                slow.duration = duration;
            }

        }
        else
        {
            buffAmount = slowAmount;
        }
    }
}
