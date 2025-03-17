using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBoardManager : MonoBehaviour
{

    public Scrollbar scoreScrollbar;  
    public TextMeshProUGUI scoreText;

    public int previousThreshold = 0;
    public int currentScore = 0;
    public int targetScore = 100;


    void UpdateScrollbarAndText()
    {
        if (targetScore > 0)
        {
            float progess = currentScore - previousThreshold;
            float totalGap = targetScore - previousThreshold;


            float scrollbarValue = progess / totalGap;
            scoreScrollbar.size = Mathf.Clamp01(scrollbarValue);
        }
        scoreText.text = $"Score: {currentScore}";
    }

    void Update(){
        Debug.Log(" GOT HERE");
        UpdateScrollbarAndText();
    }


    public void IncrementScore(int incrementAmount)
    {
        currentScore += incrementAmount;
        if (currentScore > targetScore)
        {
            currentScore = targetScore;
        }
        UpdateScrollbarAndText();
    }
}
