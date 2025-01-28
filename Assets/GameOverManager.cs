using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : NetworkBehaviour
{

    public NetworkVariable<int> readyPlayers;
    public Button restartButton;
    public TextMeshProUGUI readyAmountText;

    public GameObject defaultProjectile;

    public GameObject wall;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        readyPlayers.OnValueChanged += UpdateText;
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    void IncrementReadyPlayersServerRpc()
    {
        readyPlayers.Value++;
        if(readyPlayers.Value >= 2)
        {
            ResetClientRpc();
        }
    }

    private void UpdateText(int previousValue, int newValue)
    {
        readyAmountText.text = readyPlayers.Value.ToString() + "/2";
    }

    [ClientRpc]
    void ResetClientRpc()
    {
        wall.GetComponent<WallHealth>().wallHealth.Value = 5000f;

        EnemySpawnManager.Instance.waveTimerText.gameObject.SetActive(false);

        EnemySpawnManager.Instance.waveCount = 1;
        EnemySpawnManager.Instance.spawnInterval = 1f;

        if(EnemySpawnManager.Instance.spawnedEnemiesList.Count > 0)
        {
            for(int i = 0; i < EnemySpawnManager.Instance.spawnedEnemiesList.Count; i++)
            {
                Destroy(EnemySpawnManager.Instance.spawnedEnemiesList[i].gameObject);
            }
            EnemySpawnManager.Instance.spawnedEnemiesList.Clear();
        }

        Buff[] buffs = EnemySpawnManager.Instance.localPlayer.GetComponents<Buff>();
        for (int i = 0; i < buffs.Length; i++) 
        {
            Destroy(buffs[i]);
        }

        Debuff[] debuffs = EnemySpawnManager.Instance.localPlayer.GetComponents<Debuff>();

        for (int i = 0; i < debuffs.Length; i++)
        {
            Destroy(debuffs[i]);
        }

        TurretController turret = EnemySpawnManager.Instance.localPlayer.GetComponent<TurretController>();

        turret.fireRate = 1f;
        turret.damage = 50f;
        turret.rotationSpeed = 90f;
        turret.reloadTime = 3f;
        turret.projectile = defaultProjectile;
        turret.shootPointCount = 1;
        turret.maxAmmoCount = 10;
        turret.ammoCount = turret.maxAmmoCount;

        EnemySpawnManager.Instance.gameOverUI.SetActive(false);

        EnemySpawnManager.Instance.isGameOver = false;
        EnemySpawnManager.Instance.canSpawn = true;
    }

    public void RestartPressed()
    {
        restartButton.interactable = false;
        IncrementReadyPlayersServerRpc();
    }
}
