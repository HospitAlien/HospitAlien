using TMPro;
using UnityEngine;

public class NormalKey : MonoBehaviour
{
    public TextMeshProUGUI Text_Cap;
    public TextMeshProUGUI Text_Lower;
    public KeyboardManager keyboardManager;
    private bool isCapInput = false;

    void Awake()
    {
        if (keyboardManager != null)
        {
            keyboardManager.OnCapInputChangedEvent += OnCapInputChanged;
        }
    }

    private void OnCapInputChanged(bool isCapInput)
    {
        this.isCapInput = isCapInput;
        UpdateKeyText();
    }

    private void UpdateKeyText()
    {
        if (isCapInput)
        {
            Text_Cap.gameObject.SetActive(true);
            Text_Lower.gameObject.SetActive(false);
        }
        else
        {
            Text_Cap.gameObject.SetActive(false);
            Text_Lower.gameObject.SetActive(true);
        }
    }

    public void OnKeyClick()
    {
        if (keyboardManager != null)
        {
            keyboardManager.KeyboardInput(isCapInput ? Text_Cap.text : Text_Lower.text);
        }
    }

    void OnDestroy()
    {
        keyboardManager.OnCapInputChangedEvent -= OnCapInputChanged;
    }
}
