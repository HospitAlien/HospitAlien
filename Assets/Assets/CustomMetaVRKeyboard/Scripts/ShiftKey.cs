using UnityEngine;
using UnityEngine.UI;

public class ShiftKey : MonoBehaviour
{
    public GameObject ImageLower;
    public Image ImageLowerIcon;
    public GameObject ImageCap;

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
            ImageLower.SetActive(false);
            ImageCap.SetActive(true);
        }
        else
        {
            ImageCap.SetActive(false);
            ImageLower.SetActive(true);
            if (isCapInput) ImageLowerIcon.color = Color.green;
            else ImageLowerIcon.color = Color.white;
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
