using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SabotageThrower : NetworkBehaviour
{
    [SerializeField] SabotageProjectile snowballPrefab;
    [SerializeField] SabotageProjectile bananaPrefab;
    [SerializeField] float pickupRange = 2.5f;

    public NetworkVariable<int> HeldItem = new NetworkVariable<int>(0);

    bool IsBombHolder => BombManager.instance != null && BombManager.instance.HolderId.Value == OwnerClientId;

    void Update()
    {
        if (!IsOwner || Camera.main == null) return;

        Keyboard k = Keyboard.current;
        Mouse m = Mouse.current;

        if (k != null && k.eKey.wasPressedThisFrame) PickupRpc();

        if (m != null && m.leftButton.wasPressedThisFrame && HeldItem.Value != 0)
            ThrowRpc(GetAimDirection());
    }

    Vector3 GetAimDirection()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        float skip = Vector3.Distance(cam.transform.position, transform.position) + 1f;
        Vector3 start = ray.origin + ray.direction * skip;
        Vector3 target = ray.origin + ray.direction * 60f;

        if (Physics.Raycast(start, ray.direction, out RaycastHit hit, 60f, ~0, QueryTriggerInteraction.Ignore))
            target = hit.point;

        return (target - transform.position).normalized;
    }

    [Rpc(SendTo.Server)]
    void PickupRpc()
    {
        BombManager bomb = BombManager.instance;
        if (bomb == null || !bomb.IsArmed.Value) return;
        if (bomb.HolderId.Value == OwnerClientId) return;
        if (HeldItem.Value != 0) return;
        if (!GetComponent<PlayerState>().IsAlive.Value) return;

        SabotagePickup pickup = SabotagePickup.FindNear(transform.position, pickupRange);
        if (pickup == null || !pickup.NetworkObject.IsSpawned) return;

        HeldItem.Value = pickup.type;
        pickup.NetworkObject.Despawn();
    }

    [Rpc(SendTo.Server)]
    void ThrowRpc(Vector3 aim)
    {
        BombManager bomb = BombManager.instance;
        if (bomb == null || !bomb.IsArmed.Value) return;
        if (bomb.HolderId.Value == OwnerClientId) return;
        if (!GetComponent<PlayerState>().IsAlive.Value) return;
        if (HeldItem.Value == 0) return;
        if (aim.sqrMagnitude < 0.01f) return;

        int type = HeldItem.Value;
        SabotageProjectile prefab = type == 1 ? snowballPrefab : bananaPrefab;
        if (prefab == null) return;

        HeldItem.Value = 0;
        Vector3 dir = aim.normalized;

        SabotageProjectile p = Instantiate(prefab, transform.position + dir, Quaternion.identity);
        p.type = type;
        p.Launch(dir);
        p.NetworkObject.Spawn();
    }

    void OnGUI()
    {
        if (!IsOwner || !IsSpawned || IsBombHolder) return;

        float cx = Screen.width / 2f;
        float cy = Screen.height / 2f;
        GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 18 };

        if (HeldItem.Value != 0)
        {
            GUI.DrawTexture(new Rect(cx - 4, cy - 4, 8, 8), Texture2D.whiteTexture);
            string itemName = HeldItem.Value == 1 ? "Snowball" : "Banana";
            GUI.Label(new Rect(cx - 150, cy + 20, 300, 30), itemName + " (click to throw)", style);
        }
        else if (SabotagePickup.FindNear(transform.position, pickupRange) != null)
        {
            GUI.Label(new Rect(cx - 150, cy + 20, 300, 30), "Press E to pick up", style);
        }
    }
}