using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBoardManager : MonoBehaviour
{

    public Scrollbar scoreScrollbar;  
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI currentRank;

    private int[] targetScores;
    private string[] targetRanks;
    private int currentThreshhold;
    
    public int currentScore = 0;
    
    void Start()
    {
        targetScores = new int[] {0,2500,5000,7500,10000,12500,17500};
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
        if(currentThreshhold != targetRanks.Length -1){
            float progess = currentScore - targetScores[currentThreshhold];
            float totalGap = targetScores[currentThreshhold+1] - targetScores[currentThreshhold];
            float scrollbarValue = progess / totalGap;
            scoreScrollbar.size = Mathf.Clamp01(scrollbarValue);
        }else{
            scoreScrollbar.size = 1;
        }
        scoreText.text = $"Score: {currentScore}";
    }

    void Update(){
        if(currentThreshhold<targetScores.Length-1)
        {   
            if(targetScores[currentThreshhold+1] <= currentScore){ //if we have reached the next level
                currentThreshhold++;
                currentRank.text = targetRanks[currentThreshhold];
            }
        }
        if(currentThreshhold != 0){
            if(targetScores[currentThreshhold] > currentScore){ //if we have reached the next level
                currentThreshhold--;
                currentRank.text = targetRanks[currentThreshhold];
            }
        }
        UpdateScrollbarAndText();
    }


    public void IncrementScore(int incrementAmount)
    {
        currentScore += incrementAmount;
        if(targetScores[currentThreshhold+1] < currentScore){ //if we have reached the next level
            currentThreshhold++;
        }
        UpdateScrollbarAndText();
    }
}
