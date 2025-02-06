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
    public TextMeshProUGUI joinCodeText;
    public GameObject popUp;
    GameObject hostingPopUp;
    GameObject joiningPopUp;

    public bool startWithRelay = false;

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
            joinCodeText.gameObject.SetActive(true);
            joinCodeText.text = joinCode;
            if(hostingPopUp != null)
            {
                Destroy(hostingPopUp);
            }

            EnemySpawnManager.Instance.waveNumberText.gameObject.SetActive(true);
        }
        catch(RelayServiceException e)
        {
            GameObject hostmsg = Instantiate(popUp, transform);
            hostmsg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = e.ToString();
            Destroy(hostmsg, 3f);
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
            Destroy(joiningPopUp);
            EnemySpawnManager.Instance.waveNumberText.gameObject.SetActive(true);
        }
        catch (RelayServiceException e) 
        {
            Destroy(joiningPopUp);
            if (e.Message.Contains("invalid") || e.Message.Contains("expired"))
            {
                GameObject jcmsg = Instantiate(popUp, transform);
                jcmsg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Invalid or Expired Join Code";
                Destroy(jcmsg, 3f);
            }
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut();
            }
        }

    }

    public void Host()
    {
        if (startWithRelay)
        {
            hostingPopUp = Instantiate(popUp, transform);
            hostingPopUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Hosting...";
            StartHostWithRelay();
        }
        else
        {
            NetworkManager.Singleton.StartHost();
            hostButton.SetActive(false);
            joinButton.SetActive(false);
            startButton.SetActive(true);
            playersJoinedText.gameObject.SetActive(true);
            EnemySpawnManager.Instance.waveNumberText.gameObject.SetActive(true);
        }
        

    }

    public void Join()
    {
        if (startWithRelay)
        {
            string input = inputField.text.ToUpper();
            joiningPopUp = Instantiate(popUp, transform);
            joiningPopUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Joining...";
            JoinRelay(input);
        }
        else
        {
            NetworkManager.Singleton.StartClient();
            hostButton.SetActive(false);
            joinButton.SetActive(false);
            playersJoinedText.gameObject.SetActive(true);
            EnemySpawnManager.Instance.waveNumberText.gameObject.SetActive(true);
        }


    }

}
