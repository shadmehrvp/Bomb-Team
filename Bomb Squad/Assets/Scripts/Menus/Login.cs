using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_Text status;
    [SerializeField] private Button login;


    private async void Start()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (AuthenticationService.Instance.IsSignedIn)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        if(!AuthenticationService.Instance.SessionTokenExists) return;
        status.text = "Signing in";
        try
        {
            await 
            AuthenticationService.Instance.SignInAnonymouslyAsync();
            SceneManager.LoadScene("MainMenu");

        }
        catch
        {
            AuthenticationService.Instance.ClearSessionToken();
            status.text = "";
        }

    }
    public async void OnLoginClicked()
    {
        if (string.IsNullOrWhiteSpace(username.text) || string.IsNullOrWhiteSpace(password.text))
        {
            status.text = "Fill in all fields";
            return;
        }
        login.interactable = false;
        status.text = "Logging in....." ;

        try
        {
            await
            AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username.text , password.text);
            SceneManager.LoadScene("MainMenu");
        }

        catch (AuthenticationException)
        {
            status.text = " Wrong username or password";
        }

        catch (RequestFailedException)
        {
            status.text = "Login failed : Check your internet Connection or details";
        }

        finally
        {
            login.interactable= true;
        }
    }


    public void OnRegisterClicked()
    {
        SceneManager.LoadScene("Register");
    }


    
}
