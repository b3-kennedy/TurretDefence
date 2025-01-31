using UnityEngine;

public class SlowEffectBuff : Buff
{
    public float slowAmount;
    public float duration;


    private void Start()
    {
        Apply();
    }

    public override void Apply()
    {
        if (GetComponent<TurretController>())
        {

            var slow = gameObject.AddComponent<SlowEffect>();
            slow.slowAmount = slowAmount;
            slow.duration = duration;
        }
        else
        {
            buffAmount = slowAmount;
        }
    }
}
