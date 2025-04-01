using System.Collections;
using TMPro;
using UnityEngine;

public class LoginMenuManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI statusText;

    public GlobalVariableManager gvm;
    public PassthroughProvider passthroughProvider;

    public void Update()
    {
        passthroughProvider.TogglePassThrough(true);
        if (gvm.currentUser != null)
        {
            statusText.text = "Logged in as: " + gvm.currentUser.Email;
            Debug.Log("User is logged in: " + gvm.currentUser.Email);
            StopAllCoroutines(); // Stop the coroutine if the user is logged in
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Interactive_tutorial");
        }
    }

    public void SkipLogin()
    {
        Debug.Log("Skipping login, loading scene: Interactive_tutorial");
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Interactive_tutorial");
    }

    public void HandleLogin()
    {
        statusText.text = "Logging in...";
        string email = emailInput.text;
        string password = passwordInput.text;
        gvm.LoginUser(email, password);
        StartCoroutine(Wait5Second());
    }

    private IEnumerator Wait5Second()
    {
        yield return new WaitForSeconds(5f);
        if (gvm.currentUser == null)
        {
            statusText.text = "Login failed. Please try again.";
        }
        else
        {
            statusText.text = "Logged in as: " + gvm.currentUser.Email;
        }
    }
}