using UnityEngine;

public class DestroyTime : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

}
