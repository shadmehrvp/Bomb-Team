using TMPro;
using Unity.Netcode;
using UnityEngine;

public class JoinCode : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCode;



    private void Start()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            gameObject.SetActive(false);
            return;
        }
        
        if (!string.IsNullOrEmpty(MainMenu.JoinCode))
        {
            joinCode.text = "Join Code: " + MainMenu.JoinCode ;
        }
    }

}
