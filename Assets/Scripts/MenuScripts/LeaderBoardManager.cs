using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LeaderBoardManager : MonoBehaviour
{
    public List<GameObject> leaderboardItems;
    public GameObject LoadingSign;
    private GlobalVariableManager gvm;

    void Awake()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        gvm.OnLeaderboardLoadedEvent += OnLeaderboardLoaded;
        OnLeaderboardLoaded(gvm.LeaderBoardData);
    }

    private void OnLeaderboardLoaded(List<ScoreEntry> newScores)
    {
        UpdateLeaderBoard(newScores);
        LoadingSign.SetActive(false);
    }

    public void UpdateLeaderBoard(List<ScoreEntry> newScores)
    {

        for (int i = 0; i < newScores.Count && i < leaderboardItems.Count; i++)
        {
            leaderboardItems[i].SetActive(true);
            TextMeshProUGUI nameText = leaderboardItems[i].transform.Find("Text_Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = leaderboardItems[i].transform.Find("Text_Score").GetComponent<TextMeshProUGUI>();

            nameText.text = newScores[i].name;
            scoreText.text = newScores[i].score.ToString();
        }
        for (int i = newScores.Count; i < leaderboardItems.Count; i++)
        {
            leaderboardItems[i].SetActive(false);
        }
    }

    public void RefreshLeaderBoard()
    {
        StartCoroutine(gvm.LoadLeaderboardData());
        LoadingSign.SetActive(true);
    }

    void OnDestroy()
    {
        gvm.OnLeaderboardLoadedEvent -= OnLeaderboardLoaded;
    }
}


