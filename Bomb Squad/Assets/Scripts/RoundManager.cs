using UnityEngine;
using Unity.Netcode;

public class RoundManager : NetworkBehaviour
{
    [SerializeField] private int minimumPlayers = 2;
    private bool isRoundStarted ;


    private void Update()
    {
        if(!IsServer) return;
        if(isRoundStarted) return;
        if(BombManager.instance == null) return;
        if(NetworkManager.ConnectedClientsList.Count < minimumPlayers) return;
        int players = Random.Range(0 , NetworkManager.ConnectedClientsList.Count);
        ulong randomClientId = NetworkManager.ConnectedClientsList[players].ClientId;
        BombManager.instance.Arm(randomClientId);
        isRoundStarted = true;

    }

}
