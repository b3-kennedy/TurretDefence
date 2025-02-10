using UnityEngine;

public class RerollCountBuff : Buff
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
            UpgradeManager.Instance.maxNumberOfRerolls++;
            UpgradeManager.Instance.numberOfRerolls = UpgradeManager.Instance.maxNumberOfRerolls;
        }
    }
}
