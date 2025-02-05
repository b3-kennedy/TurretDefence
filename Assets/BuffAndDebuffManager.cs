using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class BuffAndDebuffManager : NetworkBehaviour
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

    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI reloadSpeedText;
    public TextMeshProUGUI turnRateText;
    public TextMeshProUGUI ammoChangeText;

    public Transform buffsParent;
    public GameObject textPrefab;

    public TextMeshProUGUI contentTitle;
    public GameObject button;

    public Button statsButton;

    public bool showBuff;

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

        statsButton.onClick.AddListener(ShowPanel);
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
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Tab) && !panel.activeSelf)
        {
            panel.SetActive(true);
            tc.canShoot = false;

            CalculateStats();

            damageText.text = "Damage: " + damagePercent.ToString("F1") +"%";
            fireRateText.text = "Fire Rate: " + fireRatePercent.ToString("F1")+"%";
            reloadSpeedText.text = "Reload Speed: " + reloadSpeedPercent.ToString("F1")+"%";
            turnRateText.text = "Turn Rate: " + turnRatePercent.ToString("F1") + "%";
            ammoChangeText.text = "Ammo Change: " + ammoCountChange.ToString();
            killCountText.text = "Kills: " + GetComponent<TurretController>().killCount.Value;
            

            buffs.Clear();
            debuffs.Clear();

            buffs.AddRange(GetComponents<Buff>());

            debuffs.AddRange(GetComponents<Debuff>());

            ShowBuffsOrDebuffs();

        }
        else if (Input.GetKeyDown(KeyCode.Tab) && panel.activeSelf)
        {
            tc.canShoot = true;
            panel.SetActive(false);
        }

        if (panel.activeSelf)
        {
            EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>().canShoot = false;
        }
    }

    public void ShowPanel()
    {
        statsButton.onClick.RemoveAllListeners();
        statsButton.onClick.AddListener(HidePanel);
        tc.canShoot = false;
        panel.SetActive(true);
        tc.canShoot = false;

        CalculateStats();

        damageText.text = "Damage: " + damagePercent.ToString("F1") + "%";
        fireRateText.text = "Fire Rate: " + fireRatePercent.ToString("F1") + "%";
        reloadSpeedText.text = "Reload Speed: " + reloadSpeedPercent.ToString("F1") + "%";
        turnRateText.text = "Turn Rate: " + turnRatePercent.ToString("F1") + "%";
        ammoChangeText.text = "Ammo Change: " + ammoCountChange.ToString();
        killCountText.text = "Kills: " + GetComponent<TurretController>().killCount.Value;


        buffs.Clear();
        debuffs.Clear();

        buffs.AddRange(GetComponents<Buff>());

        debuffs.AddRange(GetComponents<Debuff>());

        ShowBuffsOrDebuffs();
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        statsButton.onClick.RemoveAllListeners();
        statsButton.onClick.AddListener(ShowPanel);
        tc.canShoot = true;
        panel.SetActive(false);
    }


    void ShowBuffsOrDebuffs()
    {

        for (int i = 0; i < buffsParent.childCount; i++)
        {
            Destroy(buffsParent.GetChild(i).gameObject);
        }

        if (showBuff)
        {
            contentTitle.text = "Buffs";
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Debuffs";
            foreach (Buff buff in buffs)
            {
                GameObject textObject = Instantiate(textPrefab, buffsParent);
                TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();

                text.text = buff.buffName + " x" + buff.count.ToString();
            }
        }
        else
        {
            contentTitle.text = "Debuffs";
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Show Buffs";
            foreach (Debuff debuff in debuffs)
            {
                GameObject textObject = Instantiate(textPrefab, buffsParent);
                TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();

                text.text = debuff.debuffName + " x" + debuff.count.ToString();
            }
        }
    }

    public void SwitchModes()
    {
        showBuff = !showBuff;
        Debug.Log("hello");
        ShowBuffsOrDebuffs();
    }
}
