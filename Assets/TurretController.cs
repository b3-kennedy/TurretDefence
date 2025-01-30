using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;




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
    public Transform shootPointParent;
    float fireCooldown = 0;

    public InputAction look;
    public InputAction fire;

    [HideInInspector] public UnityEvent reloaded;
    [HideInInspector] public UnityEvent shot;
    [HideInInspector] public UnityEvent stoppedShooting;

    bool isReloading;

    float reloadTimer;

    public GameObject reloadingTextPrefab;
    GameObject reloadingText;

    public List<GameObject> projectiles = new List<GameObject>();

    public int shootPointCount = 1;

    bool isFirstShot = true;

    [HideInInspector]
    public float firstShotMultiplier = 1f;

    bool stoppedFiring = false;

    public Buff testBuff;

    public bool canShoot = true;

    public GameObject ammoTextPrefab;
    GameObject ammoText;

    public LineRenderer lr;
    public LayerMask layer;

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
            transform.position = new Vector3(-8f, 1.54f, 0f);
            if (IsOwner)
            {
                ammoText = Instantiate(ammoTextPrefab, new Vector3(-8f, 0.9f, 0f), Quaternion.identity);
                ammoText.GetComponent<TextMeshPro>().text = "Ammo: " + ammoCount.ToString();
            }
            reloadingText = Instantiate(reloadingTextPrefab, new Vector3(-8f, transform.position.y + 0.8f, 0f), Quaternion.identity);


        }
        else if (OwnerClientId == 1)
        {
            transform.position = new Vector3(-8f, -2.4f, 0f);
            if (IsOwner)
            {
                ammoText = Instantiate(ammoTextPrefab, new Vector3(-8f, -3.15f, 0f), Quaternion.identity);
                ammoText.GetComponent<TextMeshPro>().text = "Ammo: " + ammoCount.ToString();

            }

            reloadingText = Instantiate(reloadingTextPrefab, new Vector3(-8f, transform.position.y + 0.8f, 0f), Quaternion.identity);
       

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

    [ServerRpc]
    public void ChangeProjectileServerRpc(ulong clientId, int index)
    {
        ChangeProjectileClientRpc(clientId, index);
    }

    [ClientRpc]
    void ChangeProjectileClientRpc(ulong clientId, int index)
    {
        if(NetworkManager.Singleton.LocalClientId != clientId)
        {
            projectile = projectiles[index];
        }
    }

    void Update()
    {

        RaycastHit2D hit = Physics2D.Raycast(shootPointParent.GetChild(0).position, shootPointParent.GetChild(0).right,100f,layer);
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, 1f);
        if (hit)
        {
            Vector3 hitPos = new Vector3(hit.point.x, hit.point.y, 1f);
            lr.SetPosition(0, startPos);
            lr.SetPosition(1, hitPos);
        }
        else 
        {
            Vector3 endPos = startPos + (Vector3)(shootPointParent.GetChild(0).right * 100f);
            lr.SetPosition(0, startPos);
            lr.SetPosition(1, endPos);
        }

        if (!IsOwner) return;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.AddComponent(testBuff.GetType());
        }

        if(maxAmmoCount < 0)
        {
            maxAmmoCount = 1;
        }

        if(ammoCount > maxAmmoCount)
        {
            ammoCount = maxAmmoCount;
        }

        fireCooldown -= Time.deltaTime;

        Rotate();

        if(ammoCount <= 0)
        {
            isReloading = true;
        }

        if (!fire.IsPressed() && !stoppedFiring)
        {
            stoppedShooting.Invoke();
            stoppedFiring = true;
        }


        if (fire.IsPressed() && fireCooldown <= 0 && !isReloading && canShoot)
        {
            stoppedFiring = false;
            Shoot();
            ammoCount--;
            ammoText.GetComponent<TextMeshPro>().text = "Ammo: " + ammoCount.ToString();
            shot.Invoke();
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
        ammoText.GetComponent<TextMeshPro>().text = "Ammo: " + ammoCount.ToString();
        isFirstShot = true;
        reloaded.Invoke();
        

    }


    [ServerRpc(RequireOwnership = false)]
    public void ReloadedByAllyServerRpc(ulong clientId)
    {
        ulong netObjId = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.NetworkObjectId;
        ReloadedByAllyClientRpc(clientId, netObjId);
    }

    [ClientRpc]
    void ReloadedByAllyClientRpc(ulong clientId, ulong netObjId)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(netObjId, out var playerObject))
        {
            playerObject.GetComponent<TurretController>().ammoCount = maxAmmoCount;
            playerObject.GetComponent<TurretController>().isFirstShot = true;
            Debug.Log(clientId + " reload by ally");
        }
        
        

    }

    [ServerRpc]
    public void UpdateShootPointCountServerRpc(ulong clientId)
    {
        ulong playerObjId = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.NetworkObjectId;
        UpdateShootPointCountClientRpc(clientId, playerObjId);
    }

    [ClientRpc]
    void UpdateShootPointCountClientRpc(ulong clientId, ulong netObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(netObjectId, out var playerObject))
        {
            playerObject.GetComponent<TurretController>().shootPointCount = 3;
        }
    }

    void Shoot()
    {
        //instantiate local bullet
        if (IsServer)
        {
            for (int i = 0; i < shootPointCount; i++)
            {
                GameObject bullet = Instantiate(projectile, shootPointParent.GetChild(i).position, shootPointParent.GetChild(i).rotation);
                bullet.GetComponent<Rigidbody2D>().AddForce(shootPointParent.GetChild(i).right * shootForce, ForceMode2D.Impulse);
                bullet.GetComponent<NetworkObject>().Spawn();
                if (isFirstShot)
                {
                    bullet.GetComponent<DealDamage>().damage = damage * firstShotMultiplier;
                    isFirstShot = false;
                }
                else
                {
                    bullet.GetComponent<DealDamage>().damage = damage;
                }
                bullet.GetComponent<DealDamage>().player = this;
                
            }

        }
        else
        {

            for (int i = 0; i < shootPointCount; i++)
            {
                GameObject bullet = Instantiate(projectile, shootPointParent.GetChild(i).position, shootPointParent.GetChild(i).rotation);
                bullet.GetComponent<Rigidbody2D>().AddForce(shootPointParent.GetChild(i).right * shootForce, ForceMode2D.Impulse);
                bullet.GetComponent<DealDamage>().damage = damage;
                if (isFirstShot)
                {
                    bullet.GetComponent<DealDamage>().damage = damage * firstShotMultiplier;
                    isFirstShot = false;
                }
                else
                {
                    bullet.GetComponent<DealDamage>().damage = damage;
                }
                bullet.GetComponent<DealDamage>().player = this;
                CreateBulletServerRpc(OwnerClientId);
            }
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
                for (int i = 0; i < shootPointCount; i++)
                {
                    GameObject bullet = Instantiate(projectile, playerObj.GetComponent<TurretController>().shootPointParent.GetChild(i).position, playerObj.GetComponent<TurretController>().shootPointParent.GetChild(i).rotation);
                    bullet.GetComponent<Rigidbody2D>().AddForce(playerObj.GetComponent<TurretController>().shootPointParent.GetChild(i).right * shootForce, ForceMode2D.Impulse);
                    bullet.GetComponent<DealDamage>().player = playerObj.GetComponent<TurretController>();
                }

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
            Vector3 direction = (pointerPosition - transform.position).normalized;
            Vector3 fixedPointerPosition = transform.position + direction * 5;
            float angle = Mathf.Atan2(fixedPointerPosition.y - transform.position.y, fixedPointerPosition.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
