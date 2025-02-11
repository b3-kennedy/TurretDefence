using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using TMPro;


[System.Serializable]
public class EnemyAndSpawnChance
{
    public GameObject enemy;
    public float spawnChance;
}

[System.Serializable]
public class Wave
{
    public float total;
    public List<EnemyAndSpawnChance> enemies;
}

public class EnemySpawnManager : NetworkBehaviour
{
    public GameObject defendWallText;

    public static EnemySpawnManager Instance;

    public GameObject wall;

    public GameObject gameOverUI;

    public GameObject startPanel;

    [HideInInspector] public GameObject localPlayer;

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

    public NetworkVariable<bool> canSpawn;

    public NetworkVariable<bool> isPaused = new NetworkVariable<bool>(true);

    public List<Wave> waves;

    public bool isGameOver = false;

    bool hasActivatedLowPass = false;

    bool startTextTimer;
    float textTimer;

    [Header("Effects")]
    public GameObject explosion;

    public GameObject fireEffect;

    float enemyHealth = 0;
    public float healthIncrease;
    float actualHealthIncrease;
    public int healthIncreaseOnRound;
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
        Physics2D.IgnoreLayerCollision(3, 8);
        Physics2D.IgnoreLayerCollision(3, 9);
        Physics2D.IgnoreLayerCollision(8, 9);

        for (int i = 0; i < waves.Count; i++)
        {
            waves[i].total = perWaveEnemyGaph.Evaluate(i);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateExplosionServerRpc(Vector3 position)
    {
        GameObject e = Instantiate(explosion, position, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
        e.GetComponent<NetworkObject>().Spawn();
        Destroy(e, 0.25f);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateFireServerRpc(ulong parentId, float destroyTime)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(parentId, out var parent))
        {
            GameObject spawnedFire = Instantiate(fireEffect);
            var comp = spawnedFire.AddComponent<DestroyTime>();
            comp.destroyTime = destroyTime;
            spawnedFire.GetComponent<NetworkObject>().Spawn();
            spawnedFire.GetComponent<NetworkObject>().TrySetParent(parent);
            MoveFireClientRpc(spawnedFire.GetComponent<NetworkObject>().NetworkObjectId);
            
        }
    }

    [ClientRpc]
    void MoveFireClientRpc(ulong fireId)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(fireId, out var fire))
        {
            fire.transform.localPosition = Vector3.zero;
        }
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
        canSpawn.Value = false;
        ShowUpgradeInterfaceClientRpc();
    }

    [ClientRpc]
    void ShowUpgradeInterfaceClientRpc()
    {
        localPlayer.GetComponent<TurretController>().canShoot = false;
        UpgradeManager.Instance.ShowInterface();
        Cursor.visible = true;

        //reload on wave end
        localPlayer.GetComponent<TurretController>().ResetAmmo();
    }

    [ServerRpc(RequireOwnership = false)]
    void HideUpgradeInterfaceServerRpc()
    {
        
        HideUpgradeInterfaceClientRpc();
    }

    [ClientRpc]
    void HideUpgradeInterfaceClientRpc()
    {
        AudioManager.Instance.ActivateMusicLowPass();
        UpgradeManager.Instance.HideInterface();
        Cursor.visible = true;

    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        isPaused.Value = false;
        if(NetworkManager.Singleton.ConnectedClients.Count < 2)
        {
            localPlayer.transform.position = new Vector3(-8f, 0, 0);
            localPlayer.GetComponent<TurretController>().spawnedStand.transform.position = new Vector3(localPlayer.transform.position.x + 0.031f,
                localPlayer.transform.position.y, localPlayer.transform.position.z);
            localPlayer.GetComponent<TurretController>().ammoText.transform.position = new Vector3(-8f, transform.position.y - 1.5f, -1f);
            localPlayer.GetComponent<TurretController>().reloadingText.transform.position = new Vector3(-8f, transform.position.y + 1f, -1f);

        }
        StartGameForClientServerRpc();
        canSpawn.Value = true;

    }

    [ServerRpc(RequireOwnership = false)]
    void StartGameForClientServerRpc()
    {
        StartGameForClientRpc();
    }

    [ClientRpc]
    void StartGameForClientRpc()
    {
        AudioManager.Instance.boomSource.PlayOneShot(AudioManager.Instance.boomSource.clip);
        AudioManager.Instance.musicSource.volume = 0f;
        AudioManager.Instance.audioSettingsButton.gameObject.SetActive(true);
        startTextTimer = true;
        defendWallText.SetActive(true);
        
        Destroy(defendWallText, 3f);
        startPanel.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        localPlayer.GetComponent<TurretController>().canShoot = true;
        AudioManager.Instance.ActivateMusicLowPass();
    }


    GameObject GetEnemyToSpawn()
    {
        float num = Random.Range(0, 100f);

        float cumulativeChance = 0;
        if(waveCount < 50)
        {
            for (int i = 0; i < waves[waveCount].enemies.Count; i++)
            {
                cumulativeChance += waves[waveCount].enemies[i].spawnChance;

                if (num < cumulativeChance)
                {
                    return waves[waveCount].enemies[i].enemy;
                }
            }
        }
        else
        {
            for (int j = 0; j < waves[48].enemies.Count; j++)
            {
                cumulativeChance += waves[48].enemies[j].spawnChance;

                if (num < cumulativeChance)
                {
                    return waves[48].enemies[j].enemy;
                }
            }
        }

        return enemyPrefab;
    }

    private void Update()
    {
        if (isPaused.Value) return;

        if (startTextTimer)
        {
            textTimer += Time.deltaTime;
            if(textTimer >= 3f)
            {
                AudioManager.Instance.MusicSmoothToMaxVolume();
                startTextTimer = false;
                textTimer = 0;
            }
        }

        if (waveTimerText.gameObject.activeSelf && !isGameOver)
        {
            endOfWaveTimer += Time.deltaTime;
            waveTimerText.text = (endOfWaveTime - endOfWaveTimer).ToString("F1") + " SECONDS UNTIL";
        }

        if (!IsServer) return;

        if (canSpawn.Value)
        {

            if (!hasActivatedLowPass)
            {
                AudioManager.Instance.ActivateMusicLowPass();
                hasActivatedLowPass = true;
            }

            if(spawnedEnemiesList.Count < perWaveEnemyGaph.Evaluate(waveCount))
            {
                timer += Time.deltaTime;

                if (timer >= spawnInterval)
                {
                    Vector2 spawnPos = GetSpawnPoint();
                    GameObject enemy = Instantiate(GetEnemyToSpawn(), spawnPos, Quaternion.identity);
                    enemy.GetComponent<EnemyHealth>().maxHealth += actualHealthIncrease;
                    enemy.GetComponent<NetworkObject>().Spawn();
                    enemy.GetComponent<EnemyHealth>().health.Value = enemy.GetComponent<EnemyHealth>().maxHealth;
                    if (enemy.GetComponent<RangedEnemyController>())
                    {
                        enemy.GetComponent<RangedEnemyController>().wall = wall;
                    }
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
                HideUpgradeInterfaceServerRpc();
                canSpawn.Value = true;
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
            
            hasActivatedLowPass = false;
            canSpawn.Value = false;
            ShowUpgradeInterfaceServerRpc();
            UpdateWaveCountServerRpc();
            
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
        if (waveCount % healthIncreaseOnRound == 0)
        {
            Debug.Log("Increase enemy health");
            actualHealthIncrease += healthIncrease;
        }
        UpdateWaveCountClientRpc();
    }

    [ClientRpc]
    void UpdateWaveCountClientRpc()
    {
        waveCount++;
        waveNumberText.gameObject.SetActive(true);
        waveNumberText.text = "WAVE " + waveCount.ToString();
    }

    [ServerRpc(RequireOwnership = false)]
    public void GameOverServerRpc()
    {
        canSpawn.Value = false;
        ShowGameOverUIClientRpc();
    }

    [ClientRpc]
    void ShowGameOverUIClientRpc()
    {
        isGameOver = true;
        waveNumberText.gameObject.SetActive(false);
        waveTimerText.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }
}
