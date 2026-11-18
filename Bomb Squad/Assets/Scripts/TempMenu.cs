using Unity.Netcode;
using UnityEngine;

public class TempMenu : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;

    public void Start()
    {
        startMenu.SetActive(true);
    }
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        startMenu.SetActive(false);



    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        startMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
