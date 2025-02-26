using UnityEngine;

[CreateAssetMenu(fileName = "LeaderBoard", menuName = "Hospitalien/LeaderBoard")]
public class LeaderBoardIO : ScriptableObject
{
    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;
    }

    // Save the top 15 scores
    public ScoreEntry[] topScores = new ScoreEntry[15];

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

        if (insertIndex >= 0)
        {
            for (int j = topScores.Length - 1; j > insertIndex; j--)
            {
                topScores[j].playerName = topScores[j - 1].playerName;
                topScores[j].score = topScores[j - 1].score;
            }
            topScores[insertIndex].playerName = name;
            topScores[insertIndex].score = newScore;
        }
    }
}
