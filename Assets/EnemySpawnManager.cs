using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class EnemySpawnManager : NetworkBehaviour
{

    public static EnemySpawnManager Instance;
    public GameObject startPanel;

    public GameObject localPlayer;

    public float endOfWaveTime;
    float endOfWaveTimer;

    public TextMeshProUGUI waveNumberText;
    public TextMeshProUGUI waveTimerText;

    public int waveCount;
    public int spawnedEnemiesCount;

    public Transform topSpawnBoundary;
    public Transform bottomSpawnBoundary;

    public GameObject enemyPrefab;

    public AnimationCurve perWaveEnemyGaph;

    public List<GameObject> spawnedEnemiesList = new List<GameObject>();

    public float spawnInterval;
    float timer;

    public bool canSpawn;

    public NetworkVariable<bool> isPaused = new NetworkVariable<bool>(true);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        waveCount = 1;
        Physics2D.IgnoreLayerCollision(3, 3);
        Physics2D.IgnoreLayerCollision(7, 6);
        Physics2D.IgnoreLayerCollision(7, 7);
    }

    Vector2 GetSpawnPoint()
    {
        Vector2 top = topSpawnBoundary.position;
        Vector2 bottom = bottomSpawnBoundary.position;

        return new Vector2(top.x, Random.Range(bottom.y, top.y));

    }

    [ServerRpc]
    void ShowUpgradeInterfaceServerRpc()
    {
        ShowUpgradeInterfaceClientRpc();
    }

    [ClientRpc]
    void ShowUpgradeInterfaceClientRpc()
    {
        localPlayer.GetComponent<TurretController>().canShoot = false;
        UpgradeManager.Instance.ShowInterface();
    }

    [ServerRpc]
    void HideUpgradeInterfaceServerRpc()
    {
        HideUpgradeInterfaceClientRpc();
    }

    [ClientRpc]
    void HideUpgradeInterfaceClientRpc()
    {
        
        UpgradeManager.Instance.HideInterface();
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        isPaused.Value = false;
        StartGameForClientServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void StartGameForClientServerRpc()
    {
        StartGameForClientRpc();
    }

    [ClientRpc]
    void StartGameForClientRpc()
    {
        startPanel.SetActive(false);
    }

    private void Update()
    {
        if (isPaused.Value) return;

        if (waveTimerText.gameObject.activeSelf)
        {
            endOfWaveTimer += Time.deltaTime;
            waveTimerText.text = (endOfWaveTime - endOfWaveTimer).ToString("F1") + " SECONDS UNTIL";
        }

        if (!IsServer) return;

        if (canSpawn)
        {

            if (!localPlayer.GetComponent<TurretController>().canShoot)
            {
                localPlayer.GetComponent<TurretController>().canShoot = true;
            }

            if (UpgradeManager.Instance.GetInterface().activeSelf)
            {
                HideUpgradeInterfaceServerRpc();
            }

            if(spawnedEnemiesList.Count < perWaveEnemyGaph.Evaluate(waveCount))
            {
                timer += Time.deltaTime;

                if (timer >= spawnInterval)
                {
                    Vector2 spawnPos = GetSpawnPoint();
                    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                    enemy.GetComponent<NetworkObject>().Spawn();
                    spawnedEnemiesList.Add(enemy);
                    spawnedEnemiesCount++;
                    timer = 0;
                }
            }


        }
        else
        {
            ShowWaveTimerServerRpc();

            if(endOfWaveTimer >= endOfWaveTime)
            {
                HideWaveTimerServerRpc();
                canSpawn = true;
                endOfWaveTimer = 0;
                spawnInterval = 10f / perWaveEnemyGaph.Evaluate(waveCount);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void HideWaveTimerServerRpc()
    {
        HideWaveTimerClientRpc();
    }

    [ClientRpc]
    void HideWaveTimerClientRpc()
    {
        endOfWaveTimer = 0;
        waveTimerText.gameObject.SetActive(false);
    }


    [ServerRpc(RequireOwnership = false)]
    void ShowWaveTimerServerRpc()
    {
        ShowWaveTimerClientRpc();
    }

    [ClientRpc]
    void ShowWaveTimerClientRpc()
    {
        waveTimerText.gameObject.SetActive(true);

    }


    public void IsWaveOver()
    {
        int dead = 0;
        foreach (GameObject enemy in spawnedEnemiesList)
        {
            if (!enemy.activeSelf)
            {
                dead++;
            }
        }

        if(dead >= perWaveEnemyGaph.Evaluate(waveCount))
        {
            ShowUpgradeInterfaceServerRpc();
            UpdateWaveCountServerRpc();
            canSpawn = false;
            for (int i = spawnedEnemiesList.Count - 1; i >= 0; i--)
            {
                Destroy(spawnedEnemiesList[i]);
            }
            spawnedEnemiesList.Clear();
            spawnedEnemiesCount = 0;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateWaveCountServerRpc()
    {
        UpdateWaveCountClientRpc();
    }

    [ClientRpc]
    void UpdateWaveCountClientRpc()
    {
        waveCount++;
        waveNumberText.gameObject.SetActive(true);
        waveNumberText.text = "WAVE " + waveCount.ToString();
    }
}
