using System.Collections;
using UnityEngine;

public class Congratulations : MonoBehaviour
{
    public GameObject EventCanvas;
    public EventTextController eventTextController;

    private Color[] CongratulationsColors = new Color[] {
        new(1f, 0f, 0f),
        new(1f, 0.5f, 0f),
        new(1f, 1f, 0f),
        new(0f, 1f, 0f),
        new(0f, 0f, 1f),
        new(0.29f, 0f, 0.51f),
        new(0.58f, 0f, 0.83f)
    };

    private int currentColorIndex = 0;

    void Start()
    {
        ShowCongratulations();
    }

    public void ShowCongratulations()
    {
        EventCanvas.SetActive(true);
        StartCoroutine(CongratulationsTime());
    }

    public void StopCongratulations()
    {
        StopAllCoroutines();
        EventCanvas.SetActive(false);
    }


    IEnumerator CongratulationsTime()
    {
        eventTextController.SetEventText("Congratulations! You won!");
        while (true)
        {
            eventTextController.SetEventColor(CongratulationsColors[currentColorIndex]);
            currentColorIndex = (currentColorIndex + 1) % CongratulationsColors.Length;
            yield return new WaitForSeconds(1f);
        }
    }
}
