using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIManager : NetworkBehaviour
{
    public GameObject hostButton;
    public GameObject joinButton;
    public GameObject startButton;
    public GameObject panel;
    public TextMeshProUGUI playersJoinedText;

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        startButton.GetComponent<Button>().onClick.AddListener(EnemySpawnManager.Instance.StartGame);
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        playersJoinedText.text = NetworkManager.Singleton.ConnectedClients.Count.ToString() + "/2 Joined";
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        hostButton.SetActive(false);
        joinButton.SetActive(false);
        startButton.SetActive(true);
        playersJoinedText.gameObject.SetActive(true);
        
        EnemySpawnManager.Instance.waveNumberText.gameObject.SetActive(true);
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        hostButton.SetActive(false);
        joinButton.SetActive(false);
        playersJoinedText.gameObject.SetActive(true);
        EnemySpawnManager.Instance.waveNumberText.gameObject.SetActive(true);
    }

}
