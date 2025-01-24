using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System.Collections.Generic;




public class TurretController : NetworkBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed;

    [Header("Shooting")]
    public int maxAmmoCount;
    public int ammoCount;
    public float reloadTime;

    public GameObject projectile;
    public float damage;
    public float shootForce;
    public float fireRate;
    public Transform shootPoint;
    float fireCooldown = 0;

    public InputAction look;
    public InputAction fire;

    bool isReloading;

    float reloadTimer;

    public GameObject reloadingText;

    public List<GameObject> projectiles = new List<GameObject>();


    private void Awake()
    {
        //Vector3(-9.39999962,-2.4,0.0841460302)
        //Vector3(-9.39999962,2.4,0.0841460302)


    }

    public override void OnNetworkSpawn()
    {

        

        if (IsOwner)
        {
            EnemySpawnManager.Instance.localPlayer = gameObject;
            UpgradeManager.Instance.upgradeUi = transform.GetChild(2).GetChild(0).gameObject;
            UpgradeManager.Instance.cardParent = transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0);
        }

        if (OwnerClientId == 0)
        {
            transform.position = new Vector3(-8f, 2.4f, 0f);
        }
        else if (OwnerClientId == 1)
        {
            transform.position = new Vector3(-8f, -2.4f, 0f);
        }
    }

    private void OnEnable()
    {
        look.Enable();
        fire.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
        fire.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ammoCount = maxAmmoCount;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            projectile = projectiles[1];
        }

        fireCooldown -= Time.deltaTime;

        Rotate();

        if(ammoCount <= 0)
        {
            isReloading = true;
        }


        if (fire.IsPressed() && fireCooldown <= 0 && !isReloading)
        {
            Shoot();
            ammoCount--;
            fireCooldown = fireRate;
        }

        if (isReloading)
        {
            if (!reloadingText.activeSelf)
            {
                reloadingText.SetActive(true);
                ShowReloadingTextServerRpc(NetworkManager.Singleton.LocalClientId);
            }
             
            reloadTimer += Time.deltaTime;
            if(reloadTimer >= reloadTime)
            {
                reloadingText.SetActive(false);
                HideReloadingTextServerRpc(NetworkManager.Singleton.LocalClientId);
                ResetAmmo();
                isReloading = false;
                reloadTimer = 0;
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    void ShowReloadingTextServerRpc(ulong clientId)
    {
        ulong playerObjId = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkObject>().NetworkObjectId;
        ShowReloadingTextClientRpc(clientId, playerObjId);
    }

    [ClientRpc]
    void ShowReloadingTextClientRpc(ulong clientId, ulong playerObjId)
    {
        if(NetworkManager.Singleton.LocalClientId != clientId)
        {
            if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjId, out NetworkObject playerObj))
            {
                playerObj.GetComponent<TurretController>().reloadingText.SetActive(true);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void HideReloadingTextServerRpc(ulong clientId)
    {
        ulong playerObjId = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkObject>().NetworkObjectId;
        HideReloadingTextClientRpc(clientId, playerObjId);
    }

    [ClientRpc]
    void HideReloadingTextClientRpc(ulong clientId, ulong playerObjId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjId, out NetworkObject playerObj))
            {
                playerObj.GetComponent<TurretController>().reloadingText.SetActive(false);
            }
        }
    }


    public void ResetAmmo()
    {
        ammoCount = maxAmmoCount;

    }

    void Shoot()
    {
        //instantiate local bullet
        if (IsServer)
        {
            GameObject bullet = Instantiate(projectile, shootPoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().AddForce(transform.right * shootForce, ForceMode2D.Impulse);
            bullet.GetComponent<NetworkObject>().Spawn();
            bullet.GetComponent<DealDamage>().damage = damage;
        }
        else
        {
           
            GameObject bullet = Instantiate(projectile, shootPoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().AddForce(transform.right * shootForce, ForceMode2D.Impulse);
            bullet.GetComponent<DealDamage>().damage = damage;
            CreateBulletServerRpc(OwnerClientId);
        }

        
    }

    [ServerRpc(RequireOwnership = false)]
    void CreateBulletServerRpc(ulong clientid)
    {
        ulong clientPlayerId = NetworkManager.Singleton.ConnectedClients[clientid].PlayerObject.GetComponent<NetworkObject>().NetworkObjectId;

        CreateBulletClientRpc(clientid, clientPlayerId);
    }

    [ClientRpc]
    void CreateBulletClientRpc(ulong clientId, ulong networkObjectId)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var playerObj))
        {
            if (NetworkManager.Singleton.LocalClientId != clientId)
            {
                GameObject bullet = Instantiate(projectile, playerObj.GetComponent<TurretController>().shootPoint.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().AddForce(playerObj.transform.right * shootForce, ForceMode2D.Impulse);
            }
        }


    }

    void Rotate()
    {
        Vector3 pointerPosition = Vector3.zero;

        if (Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            pointerPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            pointerPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));
        }

        if (pointerPosition != Vector3.zero)
        {
            Vector3 direction = pointerPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
