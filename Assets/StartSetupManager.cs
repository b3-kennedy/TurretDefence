using TMPro;
using Unity.Netcode;
using UnityEngine;

public class StartSetupManager : NetworkBehaviour
{

    public static StartSetupManager Instance;
    public TextMeshProUGUI playersJoinedText;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }


}
