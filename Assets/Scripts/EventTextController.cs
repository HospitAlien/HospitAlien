using UnityEngine;
using TMPro;

public class EventTextController : MonoBehaviour
{
    public float scrollSpeed = 100f;
    public float textWidth = 800f;
    public string eventText;

    private float canvasNorthWidth;
    private float canvasEastWidth;
    private float canvasSouthWidth;
    private float canvasWestWidth;

    private RectTransform textNorth1;
    private RectTransform textNorth2;
    private RectTransform textEast1;
    private RectTransform textEast2;
    private RectTransform textSouth1;
    private RectTransform textSouth2;
    private RectTransform textWest1;
    private RectTransform textWest2;

    private float textPosition;
    private float totalWidthOfCanvas;

    void Start()
    {
        RectTransform canvasNorth = transform.Find("Canvas_North").GetComponent<RectTransform>();
        RectTransform canvasEast = transform.Find("Canvas_East").GetComponent<RectTransform>();
        RectTransform canvasSouth = transform.Find("Canvas_South").GetComponent<RectTransform>();
        RectTransform canvasWest = transform.Find("Canvas_West").GetComponent<RectTransform>();

        canvasNorthWidth = canvasNorth.rect.width;
        canvasEastWidth = canvasEast.rect.width;
        canvasSouthWidth = canvasSouth.rect.width;
        canvasWestWidth = canvasWest.rect.width;

        textNorth1 = canvasNorth.transform.Find("Text1").GetComponent<RectTransform>();
        textNorth2 = canvasNorth.transform.Find("Text2").GetComponent<RectTransform>();
        textEast1 = canvasEast.transform.Find("Text1").GetComponent<RectTransform>();
        textEast2 = canvasEast.transform.Find("Text2").GetComponent<RectTransform>();
        textSouth1 = canvasSouth.transform.Find("Text1").GetComponent<RectTransform>();
        textSouth2 = canvasSouth.transform.Find("Text2").GetComponent<RectTransform>();
        textWest1 = canvasWest.transform.Find("Text1").GetComponent<RectTransform>();
        textWest2 = canvasWest.transform.Find("Text2").GetComponent<RectTransform>();

        totalWidthOfCanvas = canvasNorthWidth + canvasEastWidth + canvasSouthWidth + canvasWestWidth;

        SetTextWidth(textWidth);
        SetEventText(eventText);
        InitAllText();
    }

    void Update()
    {
        moveText();
        textNorth2.anchoredPosition = new Vector2((canvasWestWidth - (textWest1.anchoredPosition.x + textWidth)), textNorth2.anchoredPosition.y);
    }

    void moveText()
    {
        textPosition += scrollSpeed * Time.deltaTime;
        textPosition = textPosition % totalWidthOfCanvas;
    }

    void InitAllText()
    {
        textPosition = 0;
        ResetTextPosition(textNorth1);
        ResetTextPosition(textNorth2, true);
        ResetTextPosition(textEast1);
        ResetTextPosition(textEast2, true);
        ResetTextPosition(textSouth1);
        ResetTextPosition(textSouth2, true);
        ResetTextPosition(textWest1);
        ResetTextPosition(textWest2, true);
    }

    void ResetTextPosition(RectTransform textRect, bool hide = false)
    {
        if (hide) textRect.anchoredPosition = new Vector2(-textWidth, textRect.anchoredPosition.y);
        else textRect.anchoredPosition = new Vector2(0, textRect.anchoredPosition.y);
    }

    public void SetTextWidth(float width)
    {
        // Make sure the width of four canvas are all bigger than the width of text
        if (canvasNorthWidth < width || canvasEastWidth < width || canvasSouthWidth < width || canvasWestWidth < width)
        {
            Debug.LogError("The width of one canvas is smaller than the width of text, controller will be disabled!");
            enabled = false;
        }
        textWidth = width;
        textNorth1.sizeDelta = new Vector2(textWidth, textNorth1.sizeDelta.y);
        textNorth2.sizeDelta = new Vector2(textWidth, textNorth2.sizeDelta.y);
        textEast1.sizeDelta = new Vector2(textWidth, textEast1.sizeDelta.y);
        textEast2.sizeDelta = new Vector2(textWidth, textEast2.sizeDelta.y);
        textSouth1.sizeDelta = new Vector2(textWidth, textSouth1.sizeDelta.y);
        textSouth2.sizeDelta = new Vector2(textWidth, textSouth2.sizeDelta.y);
        textWest1.sizeDelta = new Vector2(textWidth, textWest1.sizeDelta.y);
        textWest2.sizeDelta = new Vector2(textWidth, textWest2.sizeDelta.y);
    }

    public void SetEventText(string text)
    {
        eventText = text;
        textNorth1.GetComponent<TextMeshProUGUI>().text = eventText;
        textNorth2.GetComponent<TextMeshProUGUI>().text = eventText;
        textEast1.GetComponent<TextMeshProUGUI>().text = eventText;
        textEast2.GetComponent<TextMeshProUGUI>().text = eventText;
        textSouth1.GetComponent<TextMeshProUGUI>().text = eventText;
        textSouth2.GetComponent<TextMeshProUGUI>().text = eventText;
        textWest1.GetComponent<TextMeshProUGUI>().text = eventText;
        textWest2.GetComponent<TextMeshProUGUI>().text = eventText;
        InitAllText();
    }
}