using Unity.Netcode;
using UnityEngine;

public class RangedEnemyController : NetworkBehaviour
{
    Rigidbody2D rb;
    public float speed;
    [HideInInspector] public GameObject wall;
    public float range;
    public GameObject projectile;
    public Transform projectileSpawn;
    public float projectileForce;
    public float shootInterval;
    public float projectileDamage;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        if(Vector2.Distance(transform.position, wall.transform.position) <= range)
        {
            rb.linearVelocity = Vector2.zero;
            timer += Time.deltaTime;
            if(timer >= shootInterval)
            {
                CreateProjectileServerRpc();
                timer = 0;
            }
        }
        else
        {
            rb.linearVelocity = -transform.right * speed;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    void CreateProjectileServerRpc()
    {
        GameObject proj = Instantiate(projectile, projectileSpawn.position, Quaternion.identity);
        proj.GetComponent<NetworkObject>().Spawn();
        proj.GetComponent<EnemyProjectile>().damage = projectileDamage;
        proj.GetComponent<Rigidbody2D>().AddForce(-transform.right * projectileForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (!IsServer) return;

        if (other.collider.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
            EnemySpawnManager.Instance.IsWaveOver();
        }
    }
}
