using Unity.Netcode;
using UnityEngine;

public class Heal : NetworkBehaviour
{

    public float healAmount;
    public float healInterval;
    public float healRadius;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        timer += Time.deltaTime;
        if(timer >= healInterval)
        {
            GetComponent<EnemyHealth>().maxHealth += healAmount;
            GetComponent<EnemyHealth>().health.Value += healAmount;
            GetComponent<EnemyHealth>().UpdateScale();
            timer = 0;
        }
    }
}
