using Unity.Netcode;
using UnityEngine;

public class TempMenu : MonoBehaviour
{
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();

    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}
