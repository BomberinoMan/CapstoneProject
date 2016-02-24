using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    public InputField username;
    public InputField password;
    public Button loginButton;
    public Button BackButton;

    public void OnEnable()
    {
        loginButton.onClick.RemoveAllListeners();
        loginButton.onClick.AddListener(LoginButton_OnClick);

        BackButton.onClick.RemoveAllListeners();
        BackButton.onClick.AddListener(BackButton_OnClick);
    }

    public void LoginButton_OnClick()
    {
        //get input
        //TODO: authenticate with database

        SceneManager.LoadScene("MatchmakingLobby");
    }

    public void BackButton_OnClick()
    {
        MenuManager._instance.ChangePanel(MenuManager._instance.startupGui);
    }
}
