using System;
using TMPro;
using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    public GameObject keyboardPanel;
    public TMP_InputField inputField;

    private bool isCapsLockOn = false;
    private bool isCapInput = false;

    public void ToggleCapsLock()
    {
        isCapsLockOn = !isCapsLockOn;
        OnCapsLockInputChangedEvent?.Invoke(isCapsLockOn);
        if (isCapsLockOn)
        {
            isCapInput = true;
            OnCapInputChangedEvent?.Invoke(isCapInput);
        }
        else
        {
            isCapInput = false;
            OnCapInputChangedEvent?.Invoke(isCapInput);
        }
    }

    public void ToggleCapInput()
    {
        isCapInput = !isCapInput;
        OnCapInputChangedEvent?.Invoke(isCapInput);
    }

    public void KeyboardInput(string character)
    {
        if (inputField != null)
        {
            inputField.text += character;
        }
        if (isCapInput && !isCapsLockOn)
        {
            isCapInput = false;
            OnCapInputChangedEvent?.Invoke(isCapInput);
        }
    }

    public void BackspaceInput()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public void EnterInput()
    {
        if (inputField != null)
        {
            HideKeyboard();
        }
    }

    public event Action<bool> OnCapInputChangedEvent;
    public event Action<bool> OnCapsLockInputChangedEvent;

    public void ShowKeyboard()
    {
        keyboardPanel.SetActive(true); // 显示键盘
    }

    public void HideKeyboard()
    {
        keyboardPanel.SetActive(false); // 隐藏键盘
    }
}