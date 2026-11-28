using System;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Transform list;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private TMP_Text status;

    public void OnOpenClicked()
    {
        leaderboardPanel.SetActive(true);
        Load();
    }

    public void OnCloseClicked()
    {
        leaderboardPanel.SetActive(false);
    }

    private async void Load()
    {
       foreach(Transform texts in list)
        {
            Destroy(texts.gameObject);
        }
        
        status.text = "Loading....";

            try
            {
                var scores = await LeaderboardsService.Instance.GetScoresAsync("Wins" , new GetScoresOptions{Limit = 10});
                if (scores.Results.Count == 0)
            {
                status.text = "No Wins Yet";
                return;
            }
            status.text = "";
            foreach(var entry in scores.Results)
            {
                GameObject row = Instantiate(rowPrefab , list);
                row.GetComponent<LeaderboardRow>().SetData(entry.Rank + 1 , entry.PlayerName , (int)entry.Score);
            }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                status.text = "Failed To Load";
            }

    }
}
