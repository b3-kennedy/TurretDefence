using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
    Rigidbody2D rb;
    public float speed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = -transform.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.collider.CompareTag("Wall"))
        {
            if (IsServer)
            {
                other.gameObject.GetComponent<WallHealth>().TakeDamageServerRpc(GetComponent<EnemyHealth>().maxHealth);
            }
            
            gameObject.SetActive(false);
            EnemySpawnManager.Instance.IsWaveOver();
        }
    }
}
