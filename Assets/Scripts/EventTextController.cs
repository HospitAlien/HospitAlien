using UnityEngine;
using TMPro;
using UnityEngine.UI;


// How to use:
// 1. Set the text of the event in the EventTextController component. (via public functions)
// 2. Set the color of the event text in the EventTextController component. (via public functions)
// 3. Enable the Event_Texts_Area object.
// 4. Once event finish, disable the Event_Texts_Area object.
public class EventTextController : MonoBehaviour
{
    public float scrollSpeed = 100f;
    public float textWidth = 800f;
    public string eventText;
    public Color eventTextColor = Color.white;

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

    public TextMeshProUGUI[] TMPCollection;
    public Image[] ImageCollection;

    private float textPositionNS;
    private float textPositionEW;
    private float totalLengthOfMove;

    void Awake()
    {
        RectTransform canvasNorth = transform.Find("Canvas_North").GetComponent<RectTransform>();
        RectTransform canvasEast = transform.Find("Canvas_East").GetComponent<RectTransform>();
        RectTransform canvasSouth = transform.Find("Canvas_South").GetComponent<RectTransform>();
        RectTransform canvasWest = transform.Find("Canvas_West").GetComponent<RectTransform>();

        canvasNorthWidth = canvasNorth.rect.width;
        canvasEastWidth = canvasEast.rect.width;
        canvasSouthWidth = canvasSouth.rect.width;
        canvasWestWidth = canvasWest.rect.width;

        textNorth1 = canvasNorth.transform.Find("TextN1").GetComponent<RectTransform>();
        textNorth2 = canvasNorth.transform.Find("TextN2").GetComponent<RectTransform>();
        textEast1 = canvasEast.transform.Find("TextE1").GetComponent<RectTransform>();
        textEast2 = canvasEast.transform.Find("TextE2").GetComponent<RectTransform>();
        textSouth1 = canvasSouth.transform.Find("TextS1").GetComponent<RectTransform>();
        textSouth2 = canvasSouth.transform.Find("TextS2").GetComponent<RectTransform>();
        textWest1 = canvasWest.transform.Find("TextW1").GetComponent<RectTransform>();
        textWest2 = canvasWest.transform.Find("TextW2").GetComponent<RectTransform>();

        totalLengthOfMove = canvasNorthWidth + canvasEastWidth;

        SetTextWidth(textWidth);
        SetEventText(eventText);
        SetEventColor(eventTextColor);
        ResetTextPosition();
    }

    void Update()
    {
        moveText();

        textNorth1.anchoredPosition = new Vector2(mapMovement(textPositionNS, canvasNorthWidth, canvasEastWidth), textNorth1.anchoredPosition.y);
        textEast1.anchoredPosition = new Vector2(mapMovement(textPositionEW, canvasEastWidth, canvasSouthWidth), textEast1.anchoredPosition.y);
        textSouth1.anchoredPosition = new Vector2(mapMovement(textPositionNS, canvasSouthWidth, canvasWestWidth), textSouth1.anchoredPosition.y);
        textWest1.anchoredPosition = new Vector2(mapMovement(textPositionEW, canvasWestWidth, canvasNorthWidth), textWest1.anchoredPosition.y);
        textNorth2.anchoredPosition = new Vector2(-canvasWestWidth + textPositionEW, textNorth2.anchoredPosition.y);
        textEast2.anchoredPosition = new Vector2(-canvasNorthWidth + textPositionNS, textEast2.anchoredPosition.y);
        textSouth2.anchoredPosition = new Vector2(-canvasEastWidth + textPositionEW, textSouth2.anchoredPosition.y);
        textWest2.anchoredPosition = new Vector2(-canvasSouthWidth + textPositionNS, textWest2.anchoredPosition.y);
    }

    float mapMovement(float position, float canvasA, float canvasB)
    {
        if (position <= canvasA) return position;
        else return -canvasB + position - canvasA;
    }

    void moveText()
    {
        textPositionEW += scrollSpeed * Time.deltaTime;
        textPositionNS += scrollSpeed * Time.deltaTime;
        textPositionEW %= totalLengthOfMove;
        textPositionNS %= totalLengthOfMove;
    }

    public void ResetTextPosition()
    {
        float difference = canvasNorthWidth - canvasEastWidth;
        if (difference > 0)
        {
            textPositionNS = difference;
            textPositionEW = 0;
        }
        else
        {
            textPositionNS = 0;
            textPositionEW = -difference;
        }
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
        if (canvasEastWidth + canvasNorthWidth != canvasSouthWidth + canvasWestWidth)
        {
            Debug.LogError("Canvas did not forms a rectangle, controller will be disabled!");
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

    public void SetEventText(string text, bool resetPosition = false)
    {
        eventText = text;
        // Set the text of all TMP
        for (int i = 0; i < TMPCollection.Length; i++)
        {
            TMPCollection[i].text = eventText;
        }
        if (resetPosition) ResetTextPosition();
    }

    public void SetEventColor(Color color, bool resetPosition = false)
    {
        eventTextColor = color;
        // Set the color of all text
        for (int i = 0; i < TMPCollection.Length; i++)
        {
            TMPCollection[i].color = eventTextColor;
        }
        // Set the color of all background image in canvas
        for (int i = 0; i < ImageCollection.Length; i++)
        {
            ImageCollection[i].color = eventTextColor;
        }
        if (resetPosition) ResetTextPosition();
    }
}