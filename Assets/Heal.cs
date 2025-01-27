using UnityEngine;

public class Heal : MonoBehaviour
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
        timer += Time.deltaTime;
        if(timer >= healInterval)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, healRadius);

            foreach(Collider2D c in enemies)
            {
                if(c != null)
                {
                    if (!c.GetComponent<Heal>())
                    {
                        c.GetComponent<EnemyHealth>().maxHealth += healAmount;
                        c.GetComponent<EnemyHealth>().health.Value += healAmount;
                        c.GetComponent<EnemyHealth>().UpdateScale();
                    }
                }

            }
            timer = 0;
        }
    }
}
