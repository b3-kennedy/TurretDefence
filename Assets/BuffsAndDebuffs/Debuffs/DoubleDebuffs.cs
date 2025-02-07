using UnityEngine;

public class DoubleDebuffs : Debuff
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
            Debuff[] debuffs = GetComponents<Debuff>();
            foreach (Debuff debuff in debuffs)
            {
                if (debuff != this)
                {
                    Debug.Log("Previous Value: " + debuff.debuffAmount);
                    debuff.debuffAmount *= 2;
                    debuff.Apply();
                    Debug.Log("Post Value: " + debuff.debuffAmount);
                }

            }
            if (GetComponent<BurnEffect>())
            {
                GetComponent<BurnEffect>().damage /= 2;
            }
        }
    }

    public override void Setup()
    {
        base.Setup();
    }
}
