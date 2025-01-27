using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{

    public static AuthenticationManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public async void InitializeAuthentication()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Player signed in with ID: {AuthenticationService.Instance.PlayerId}");
        }
    }
}
