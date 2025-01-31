using Unity.Netcode;
using UnityEngine;

public class DestroyAfterTime : NetworkBehaviour
{

    public GameObject bulletPrefab;
    public float destroyTime;
    public float spreadAngle = 90f;
    public int bulletCount = 10;
    public float bulletSpeed;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void SpawnBullets()
    {
        float angleStep = spreadAngle / (bulletCount - 1); // Angle between bullets
        float startAngle = -spreadAngle / 2;               // Starting angle

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + (angleStep * i);    // Calculate bullet angle
            Quaternion rotation = Quaternion.Euler(0, 0, angle); // 2D rotation

            // Spawn bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);
            bullet.GetComponent<DealDamage>().damage = GetComponent<DealDamage>().damage / (bulletCount / 2f);
            bullet.GetComponent<DealDamage>().player = GetComponent<DealDamage>().player;
            // Set bullet velocity
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = rotation * transform.right; // Rotate vector (1, 0)
                rb.linearVelocity = direction * bulletSpeed;
            }

            // For 3D, replace Rigidbody2D with Rigidbody and use rotation * Vector3.forward
        }
    }



    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= destroyTime)
        {
            SpawnBullets();
            if (GetComponent<NetworkObject>().IsSpawned)
            {
                if (IsServer)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}