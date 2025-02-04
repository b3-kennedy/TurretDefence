using Unity.Netcode;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{

    public float destroyTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroyTime);
        
    }

}
