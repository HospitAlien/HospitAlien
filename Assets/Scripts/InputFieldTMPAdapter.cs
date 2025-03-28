using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldTMPAdapter : MonoBehaviour
{
    public InputField hiddenInputField;
    public TMP_InputField tmpInputField;

    void Update()
    {
        if (hiddenInputField.text != tmpInputField.text)
        {
            tmpInputField.text = hiddenInputField.text;
        }

        if (tmpInputField.text != hiddenInputField.text)
        {
            hiddenInputField.text = tmpInputField.text;
        }
    }
}