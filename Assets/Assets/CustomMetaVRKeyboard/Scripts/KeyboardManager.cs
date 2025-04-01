using System;
using TMPro;
using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    public GameObject keyboardPanel;
    public inputFieldRef inputFieldRef;
    private TMP_InputField inputField;
    private bool isCapsLockOn = false;
    private bool isCapInput = false;

    void Start()
    {
        inputFieldRef.OnInputFieldChangedEvent += SetInputField;
        inputField = inputFieldRef.GetCurrentInputField();
        HideKeyboard();
    }

    void OnDestroy()
    {
        inputFieldRef.OnInputFieldChangedEvent -= SetInputField;
    }

    private void SetInputField(TMP_InputField newInputField)
    {
        inputField = newInputField;
        if (inputField != null)
        {
            ShowKeyboard();
        }
    }

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
            int caretPos = inputField.caretPosition;
            inputField.text = inputField.text.Insert(caretPos, character);
            inputField.caretPosition = caretPos + character.Length;
        }
        if (isCapInput && !isCapsLockOn)
        {
            isCapInput = false;
            OnCapInputChangedEvent?.Invoke(isCapInput);
        }
    }

    public void SpaceInput()
    {
        if (inputField != null)
        {
            int caretPos = inputField.caretPosition;
            inputField.text = inputField.text.Insert(caretPos, " ");
            inputField.caretPosition = caretPos + 1;
        }
    }

    public void BackspaceInput()
    {
        if (inputField != null && inputField.caretPosition > 0)
        {
            int caretPos = inputField.caretPosition;
            inputField.text = inputField.text.Remove(caretPos - 1, 1);
            inputField.caretPosition = caretPos - 1;
        }
    }

    public void EnterInput()
    {
        HideKeyboard();
    }

    public event Action<bool> OnCapInputChangedEvent;
    public event Action<bool> OnCapsLockInputChangedEvent;

    public void ShowKeyboard()
    {
        keyboardPanel.SetActive(true);
    }

    public void HideKeyboard()
    {
        keyboardPanel.SetActive(false);
    }
}