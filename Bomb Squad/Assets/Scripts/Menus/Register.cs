using TMPro;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
   [SerializeField] private TMP_InputField displayName;
   [SerializeField] private TMP_InputField emailAdress;
   [SerializeField] private TMP_InputField username;
   [SerializeField] private TMP_InputField password;
   [SerializeField] private TMP_InputField confirmPassword;
   [SerializeField] private TMP_Text status;
   [SerializeField] private Button registerButton;



   private async void Start()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }
        
    }

    public async void OnRegisterClicked()
    {
        if(string.IsNullOrWhiteSpace(displayName.text) || string.IsNullOrWhiteSpace(emailAdress.text) || string.IsNullOrWhiteSpace(username.text) || string.IsNullOrWhiteSpace(password.text) || string.IsNullOrWhiteSpace(confirmPassword.text))
        {
            status.text = " Please fill up all fields";
            return;
        }

        if( password.text != confirmPassword.text)
        {
            status.text = " passwords are not matched";
            return;

        }
        registerButton.interactable = false;
        status.text = " Creating account";

        try
        {
            await
            AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username.text, password.text);
            await
            AuthenticationService.Instance.UpdatePlayerNameAsync(displayName.text);
            var data = new Dictionary<string , object>{{"email" , emailAdress.text }};
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            SceneManager.LoadScene("MainMenu");
        }

        catch (AuthenticationException)
        {
            status.text = "Username already Taken or invalid";
        }

        catch (RequestFailedException)
        {
            status.text = " Password needs 8+ characters , with upper , lower , number and symbol letters ";
        }

        finally
        {
            registerButton.interactable = true;
        }
    }

    public void OnBackClick()
    {
        SceneManager.LoadScene("Login");
    }


}
