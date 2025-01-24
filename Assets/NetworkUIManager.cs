using Unity.Netcode;
using UnityEngine;

public class NetworkUIManager : MonoBehaviour
{
    public GameObject panel;

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        panel.SetActive(false);
        EnemySpawnManager.Instance.waveNumberText.gameObject.SetActive(true);
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        panel.SetActive(false);
        EnemySpawnManager.Instance.waveNumberText.gameObject.SetActive(true);
    }
}
