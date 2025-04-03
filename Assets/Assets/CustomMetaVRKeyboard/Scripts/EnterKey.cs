using TMPro;
using UnityEngine;

public class EnterKey : MonoBehaviour
{
    public KeyboardManager keyboardManager;

    public void OnKeyClick()
    {
        if (keyboardManager != null)
        {
            keyboardManager.EnterInput();
        }
    }
}
