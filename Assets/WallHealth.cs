using Unity.Netcode;
using UnityEngine;

public class WallHealth : NetworkBehaviour
{
    public float maxHealth;
    [HideInInspector] public NetworkVariable<float> wallHealth;
    public GameObject wallHealthBar;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            wallHealth.Value = maxHealth;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float dmg)
    {
        if (!IsServer) return;

        if(wallHealth.Value > 0)
        {
            wallHealth.Value -= dmg;
            UpdateHealthBarClientRpc();
        }
        else
        {
            EnemySpawnManager.Instance.GameOverServerRpc();
        }
    }

    [ClientRpc]
    public void UpdateHealthBarClientRpc()
    {
        float percent = wallHealth.Value / maxHealth;
        wallHealthBar.transform.localScale = new Vector3(wallHealthBar.transform.localScale.x, percent, 1);
    }

    private void Update()
    {
        
    }
}
