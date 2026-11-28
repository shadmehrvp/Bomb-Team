
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resume;
    [SerializeField] private Button setting;
    [SerializeField] private Button leaveSession;

    public static bool IsOpen ;


    private void Start()
    {
        pauseMenu.SetActive(false);
    }


    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void OnResumeClicked()
    {
        pauseMenu.SetActive(false);
        IsOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void OnLeaveClicked()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        MainMenu.JoinCode = null;
        IsOpen = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }








}
