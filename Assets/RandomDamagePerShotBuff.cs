using UnityEngine;

public class RandomDamagePerShotBuff : Buff
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
            GetComponent<TurretController>().shot.AddListener(RandomiseDamage);
        }
    }

    void RandomiseDamage()
    {
        if (GetComponent<TurretController>())
        {
            GetComponent<TurretController>().damage = Random.Range(1f, 150f);
        }
    }
}
