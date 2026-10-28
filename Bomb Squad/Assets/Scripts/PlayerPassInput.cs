using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerPassInput : NetworkBehaviour
{
    void Update()
    {
        if(!IsOwner) return;
        Keyboard kb = Keyboard.current;
        if(kb == null) return;
        if(!kb.eKey.wasPressedThisFrame) return;
        if(BombManager.instance == null ) return;
        if(BombManager.instance.HolderId.Value != OwnerClientId) return;
        PassReqestRpc();

    


    }

    [Rpc(SendTo.Server)]
    private void PassReqestRpc(RpcParams rpcParams = default)
    {
        ulong Sender = rpcParams.Receive.SenderClientId;
        if(Sender != OwnerClientId)return;
        if(BombManager.instance == null) return;
        if (!BombManager.instance.TryPass(Sender , out string reason))
        {
            RespondRpc(reason);
        }

    }

    [Rpc(SendTo.Owner)]
    private void RespondRpc(string reason)
    {
        Debug.Log(reason);
    }
}
