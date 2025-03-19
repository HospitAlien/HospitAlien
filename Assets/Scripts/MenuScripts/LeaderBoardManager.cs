using UnityEngine;
using System.Collections.Generic;
using TMPro;

// this script should attached to game setting menu to let buttons work
public class LeaderBoardManager : MonoBehaviour
{
    public List<GameObject> leaderboardItems;

    private GlobalVariableManager gvm;
    private string scoreString = "£%0";


    void Start()
    {
        gvm = FindFirstObjectByType<GlobalVariableManager>();
        if (gvm == null) Debug.LogError("GlobalVariableManager not found");
        gvm.LeaderBoard.OnLeaderBoardChanged += UpdateLeaderBoard;

        UpdateLeaderBoard(gvm.LeaderBoard);
    }

    void UpdateLeaderBoard(LeaderBoardIO leaderBoard)
    {
        for (int i = 0; i < leaderboardItems.Count; i++)
        {
            if (leaderBoard.topScores[i] != null && leaderBoard.topScores[i].playerName != "")
            {
                leaderboardItems[i].SetActive(true);
                TextMeshProUGUI nameText = leaderboardItems[i].transform.Find("Text_Name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI scoreText = leaderboardItems[i].transform.Find("Text_Score").GetComponent<TextMeshProUGUI>();

                nameText.text = leaderBoard.topScores[i].playerName;
                scoreText.text = scoreString.Replace("%0", leaderBoard.topScores[i].score.ToString());
            }
            else
            {
                leaderboardItems[i].SetActive(false);
            }
        }
    }
}