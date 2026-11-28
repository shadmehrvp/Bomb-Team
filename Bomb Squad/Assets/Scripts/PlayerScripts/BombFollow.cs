using System;
using Unity.Netcode;
using UnityEngine;

public class BombFollow : NetworkBehaviour
{
private Transform playerCapsule;
[SerializeField] private Vector3 bombHolderPoint;
private ulong lastHolder = BombManager.NoHolder;
private Renderer bomb;

private void Start()
    {
        bomb = GetComponent<Renderer>();
    }





    private void LateUpdate()
    {
        if(!BombManager.instance) return;
        ulong holder = BombManager.instance.HolderId.Value;
        if(holder != lastHolder)
        {
            lastHolder = holder;
            playerCapsule = FindCapsule(holder);
        }
        if(playerCapsule == null)
        {
            bomb.enabled = false;
            return;
        }
        bomb.enabled = true;
        bomb.transform.position = playerCapsule.TransformPoint(bombHolderPoint);

    }

    private Transform FindCapsule(ulong holder)
    {
       if(holder == BombManager.NoHolder) return null;
       foreach(NetworkObject obj in NetworkManager.SpawnManager.SpawnedObjectsList)
        {
            if(obj.IsPlayerObject && obj.OwnerClientId == holder) return obj.transform;


        }
        return null;
    }
}
