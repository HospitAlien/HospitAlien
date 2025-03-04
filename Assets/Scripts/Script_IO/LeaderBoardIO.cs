using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LeaderBoard", menuName = "Hospitalien/LeaderBoard")]
public class LeaderBoardIO : ScriptableObject
{
    [Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;
    }

    // Save the top 10 scores
    public ScoreEntry[] topScores = new ScoreEntry[10];

    // Add a new score to the leaderboard
    public void AddNewScore(string name, int newScore)
    {
        int insertIndex = -1;
        for (int i = 0; i < topScores.Length; i++)
        {
            if (topScores[i] == null)
            {
                topScores[i] = new ScoreEntry { playerName = name, score = newScore };
                return;
            }

            if (newScore > topScores[i].score)
            {
                insertIndex = i;
                break;
            }
        }

        // If the new score is in the top 10, insert it into the array and shift the other scores down
        if (insertIndex >= 0)
        {
            for (int j = topScores.Length - 1; j > insertIndex; j--)
            {
                if (topScores[j - 1] != null)
                {
                    topScores[j] = topScores[j - 1];
                }

            }
            topScores[insertIndex] = new ScoreEntry { playerName = name, score = newScore };
            OnLeaderBoardChanged?.Invoke(this);
        }
    }

    // DEBUG ONLY: Sort the top scores
    public void SortTopScores()
    {
        System.Array.Sort(topScores, (x, y) =>
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            return y.score.CompareTo(x.score);
        });
    }

    public event Action<LeaderBoardIO> OnLeaderBoardChanged;
}
