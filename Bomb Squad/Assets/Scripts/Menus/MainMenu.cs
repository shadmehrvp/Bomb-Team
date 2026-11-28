using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text welcome;
    [SerializeField] private Button startHost;
    [SerializeField] private Button joinSession;
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text status;
    public static string JoinCode;

    private async void Start()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            SceneManager.LoadScene("Login");
            return;
        }

        welcome.text = "Welcome , "  +  AuthenticationService.Instance.PlayerName;
    }

    public async void OnHostClicked()
    {
        startHost.interactable = false;
        joinSession.interactable = false;
        status.text = "Please wait for a moment , we are creating the game lobby";

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation,"dtls"));
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Shadmehr" , LoadSceneMode.Single);
        }

        catch
        {
            status.text = "could not create the game session . Try Again.";
            startHost.interactable = true;
            joinSession.interactable = true;
        }
    }

    public async void OnJointClicked()
    {
        if (string.IsNullOrWhiteSpace(joinCodeInput.text))
        {
            status.text = "Enter a join code";
            return;
        }
        startHost.interactable = false;
        joinSession.interactable = false;
        status.text = "joining ...";

        try
        {
            JoinAllocation join = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.text.Trim().ToUpper());
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(join , "dtls"));
            NetworkManager.Singleton.StartClient(); 
        }

        catch
        {
            status.text = "Could not join . Please check the code";        
            startHost.interactable = true;
            joinSession.interactable = true;

        }
    }

    public void OnLogoutClicked()
    {
        AuthenticationService.Instance.SignOut(true);
        SceneManager.LoadScene("Login");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

}
