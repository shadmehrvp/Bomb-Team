using TMPro;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text winsText;


    public void SetData(int rank , string playerName , int wins)
    {
        rankText.text = "#" + rank;
        nameText.text = playerName;
        winsText.text = wins.ToString();
    }
}
