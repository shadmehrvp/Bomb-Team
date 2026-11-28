using Unity.Netcode;
using UnityEngine;

public class CountdownManager : NetworkBehaviour
{
    public static CountdownManager instance { get; private set; }

    public NetworkVariable<double> EndTime = new NetworkVariable<double>(0);
    public NetworkVariable<int> Mode = new NetworkVariable<int>(0);

    public override void OnNetworkSpawn() => instance = this;

    public override void OnNetworkDespawn()
    {
        if (instance == this) instance = null;
    }

    public void Show(int mode, float seconds)
    {
        if (!IsServer) return;
        Mode.Value = mode;
        EndTime.Value = NetworkManager.ServerTime.Time + seconds;
    }

    void OnGUI()
    {
        if (Event.current.type != EventType.Repaint) return;
        if (!IsSpawned || Mode.Value == 0) return;

        double left = EndTime.Value - NetworkManager.ServerTime.Time;
        if (left <= 0) return;

        string title = Mode.Value == 1 ? "Round starts in" : "Bomb given in";

        GUIStyle small = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 28 };
        GUIStyle big = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 90, fontStyle = FontStyle.Bold };

        GUI.Label(new Rect(0, 60, Screen.width, 40), title, small);
        GUI.Label(new Rect(0, 100, Screen.width, 110), Mathf.CeilToInt((float)left).ToString(), big);
    }
}
