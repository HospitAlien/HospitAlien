using TMPro;
using UnityEngine;

public class inputFieldRef : MonoBehaviour
{
    public TMP_InputField[] inputFields;

    public int currentInputFieldIndex = 0;

    public void SetInputField(int index)
    {
        if (index >= 0 && index < inputFields.Length)
        {
            currentInputFieldIndex = index;
            OnInputFieldChangedEvent?.Invoke(inputFields[currentInputFieldIndex]);
        }
        else
        {
            Debug.LogError("Index out of range for input fields.");
        }
    }

    public TMP_InputField GetCurrentInputField()
    {
        if (currentInputFieldIndex >= 0 && currentInputFieldIndex < inputFields.Length)
        {
            return inputFields[currentInputFieldIndex];
        }
        return null;
    }

    public event System.Action<TMP_InputField> OnInputFieldChangedEvent;
}