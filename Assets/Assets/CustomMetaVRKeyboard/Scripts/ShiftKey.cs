using UnityEngine;
using UnityEngine.UI;

public class ShiftKey : MonoBehaviour
{
    public Image Image_Cap;
    public Image Image_Lower;

    public KeyboardManager keyboardManager;
    private bool isCapsLockOn = false;
    private bool isCapInput = false;

    void Awake()
    {
        if (keyboardManager != null)
        {
            keyboardManager.OnCapInputChangedEvent += OnCapInputChanged;
            keyboardManager.OnCapsLockInputChangedEvent += OnCapsLockInputChanged;
        }
    }

    private void OnCapInputChanged(bool isCapInput)
    {
        this.isCapInput = isCapInput;
        UpdateKeyImage();
    }

    private void OnCapsLockInputChanged(bool isCapsLockOn)
    {
        this.isCapsLockOn = isCapsLockOn;
        UpdateKeyImage();
    }

    private void UpdateKeyImage()
    {
        if (isCapsLockOn)
        {
            Image_Cap.gameObject.SetActive(true);
            Image_Lower.gameObject.SetActive(false);
        }
        else
        {
            Image_Cap.gameObject.SetActive(false);
            Image_Lower.gameObject.SetActive(true);
            if (isCapInput) Image_Lower.color = Color.green;
            else Image_Lower.color = Color.white;
        }
    }

    public void OnKeyClick()
    {
        if (keyboardManager != null)
        {
            if (isCapsLockOn)
            {
                keyboardManager.ToggleCapsLock();
            }
            else
            {
                if (!isCapInput)
                {
                    keyboardManager.ToggleCapInput();
                }
                else
                {
                    keyboardManager.ToggleCapsLock();
                }
            }
        }
    }

    void OnDestroy()
    {
        if (keyboardManager != null)
        {
            keyboardManager.OnCapInputChangedEvent -= OnCapInputChanged;
            keyboardManager.OnCapsLockInputChangedEvent -= OnCapsLockInputChanged;
        }
    }
}
