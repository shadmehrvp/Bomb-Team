using TMPro;
using UnityEngine;

public class BombTimerText : MonoBehaviour
{
    private TextMeshPro label;

    private void Awake()
    {
        label = GetComponent<TextMeshPro>();
    }

    private void LateUpdate()
    {
        BombManager bomb = BombManager.instance;
        bool show = bomb != null && bomb.IsArmed.Value;
        label.enabled = show;
        if (!show) return;

        float timeLeft = (float)bomb.TimeLeft;
        label.text = Mathf.CeilToInt(timeLeft).ToString();
        label.color = timeLeft <= 10f ? Color.red : Color.white;

        if (Camera.main != null) transform.rotation = Camera.main.transform.rotation;
    }
}