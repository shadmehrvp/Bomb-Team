using TMPro;
using UnityEngine;

public class RoundUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundText;

    private void Update()
    {
        RoundManager roundManager = RoundManager.instance;
        if ( roundManager == null || roundManager.RoundNumber.Value == 0)
        {
            roundText.text = "";
            return;
        }

        roundText.text = "ROUND" + roundManager.RoundNumber.Value;
    }
}
