using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class NetworkUIManager : NetworkBehaviour
{
    public GameObject hostButton;
    public GameObject joinButton;
    public GameObject startButton;
    public GameObject panel;
    public TMP_InputField inputField;
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

    public async void StartHostWithRelay()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => { Debug.Log(AuthenticationService.Instance.PlayerId); };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        CreateRelay();
    }

    async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);
            NetworkManager.Singleton.StartHost();
            hostButton.SetActive(false);
            joinButton.SetActive(false);
            startButton.SetActive(true);
            playersJoinedText.gameObject.SetActive(true);

            EnemySpawnManager.Instance.waveNumberText.gameObject.SetActive(true);
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    async void JoinRelay(string joinCode)
    {
        try
        {

            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += () => { Debug.Log(AuthenticationService.Instance.PlayerId); };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            JoinAllocation joinAllocation =  await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();
            hostButton.SetActive(false);
            joinButton.SetActive(false);
            playersJoinedText.gameObject.SetActive(true);
            EnemySpawnManager.Instance.waveNumberText.gameObject.SetActive(true);
        }
        catch (RelayServiceException e) 
        {
            Debug.Log(e);
        }

    }

    public void Host()
    {
        StartHostWithRelay();

    }

    public void Join()
    {
        string input = inputField.text.ToUpper();
        JoinRelay(input);

    }

}
