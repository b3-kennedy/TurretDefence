using UnityEngine;

public class SlowEffectBuff : Buff
{
    public float slowAmount;
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
            if (!GetComponent<SlowEffect>())
            {
                var slow = gameObject.AddComponent<SlowEffect>();
                slow.slowAmount = slowAmount;
                slow.duration = duration;

                Debug.Log(slowAmount);
            }

        }
        else
        {
            buffAmount = slowAmount;
        }
    }
}
