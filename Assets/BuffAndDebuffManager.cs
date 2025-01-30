using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BuffAndDebuffManager : MonoBehaviour
{

    public List<Buff> buffs = new List<Buff>();
    public List<Debuff> debuffs = new List<Debuff>();

    public float damagePercent;
    public float fireRatePercent;
    public float reloadSpeedPercent;
    public float turnRatePercent;
    public int ammoCountChange;

    float startDamage;
    float startFireRate;
    float startReloadSpeed;
    float startTurnRate;
    int startAmmoCount;

    TurretController tc;

    public GameObject panel;

    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI reloadSpeedText;
    public TextMeshProUGUI turnRateText;
    public TextMeshProUGUI ammoChangeText;

    public Transform buffsParent;
    public GameObject textPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tc = GetComponent<TurretController>();
        startDamage = tc.damage;
        startFireRate = tc.fireRate;
        startReloadSpeed = tc.reloadTime;
        startTurnRate = tc.rotationSpeed;
        startAmmoCount = tc.maxAmmoCount;

        CalculateStats();
    }

    void CalculateStats()
    {
        damagePercent = (tc.damage / startDamage) * 100f;
        fireRatePercent = (startFireRate / tc.fireRate) * 100f;
        reloadSpeedPercent = (tc.reloadTime / startReloadSpeed) * 100f;
        turnRatePercent = (tc.rotationSpeed / startTurnRate) * 100f;
        ammoCountChange = tc.maxAmmoCount - startAmmoCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            panel.SetActive(true);

            CalculateStats();

            damageText.text = "Damage: " + damagePercent.ToString("F1") +"%";
            fireRateText.text = "Fire Rate: " + fireRatePercent.ToString("F1")+"%";
            reloadSpeedText.text = "Reload Speed: " + reloadSpeedPercent.ToString("F1")+"%";
            turnRateText.text = "Turn Rate: " + turnRatePercent.ToString("F1") + "%";
            ammoChangeText.text = "Ammo Change: " + ammoCountChange.ToString();
            

            buffs.Clear();
            debuffs.Clear();

            for (int i = 0; i < buffsParent.childCount; i++)
            {
                Destroy(buffsParent.GetChild(i).gameObject);
            }

            buffs.AddRange(GetComponents<Buff>());

            debuffs.AddRange(GetComponents<Debuff>());

            foreach (Buff buff in buffs)
            {
                GameObject textObject = Instantiate(textPrefab, buffsParent);
                TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();

                text.text = buff.buffName + " x" + buff.count.ToString();
            }

        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            panel.SetActive(false);
        }
    }
}
