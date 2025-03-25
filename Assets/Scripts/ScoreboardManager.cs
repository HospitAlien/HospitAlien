using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBoardManager : MonoBehaviour
{

    public Scrollbar scoreScrollbar;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI currentRank;
    public TextMeshProUGUI currentTime;
    private int[] targetScores;
    private string[] targetRanks;

    private int currentThreshhold;
    private int currentScore = 0;

    void Start()
    {
        targetScores = new int[] { 0, 2500, 5000, 7500, 10000, 12500, 17500 };
        targetRanks = new string[]
        {
            "Medical Student",
            "Resident",
            "Fellow",
            "Attending Physician",
            "Department Head",
            "Medical Director",
            "Consultant Doctor"
        };
        currentThreshhold = 0;
        currentRank.text = targetRanks[currentThreshhold];
    }

    void UpdateScrollbarAndText()
    {
        if (currentThreshhold != targetRanks.Length - 1)
        {
            float progess = currentScore - targetScores[currentThreshhold];
            float totalGap = targetScores[currentThreshhold + 1] - targetScores[currentThreshhold];
            float scrollbarValue = progess / totalGap;
            scoreScrollbar.size = Mathf.Clamp01(scrollbarValue);
        }
        else
        {
            scoreScrollbar.size = 1;
        }
        scoreText.text = $"Score: {currentScore}";
    }

    public void setTime(float newTime)
    {
        int minutes = (int)(newTime / 60);
        int seconds = (int)(newTime % 60);
        currentTime.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void SetTimeBeforeStart(float newTime)
    {
        int minutes = (int)(newTime / 60);
        int seconds = (int)(newTime % 60);
        currentTime.text = $"Game start in: {minutes:D2}:{seconds:D2}";
    }

    public void HideTime()
    {
        currentTime.enabled = false;
    }


    public void setScore(int newScore)
    {
        currentScore = newScore;
        if (currentThreshhold < targetScores.Length - 1)
        {
            if (targetScores[currentThreshhold + 1] <= currentScore)
            { //if we have reached the next level
                currentThreshhold++;
                currentRank.text = targetRanks[currentThreshhold];
            }
        }
        if (currentThreshhold != 0)
        {
            if (targetScores[currentThreshhold] > currentScore)
            { //if we have fallen below current level
                currentThreshhold--;
                currentRank.text = targetRanks[currentThreshhold];
            }
        }
        UpdateScrollbarAndText();
    }
}
