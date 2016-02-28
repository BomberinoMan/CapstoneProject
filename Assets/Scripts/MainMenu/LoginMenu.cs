using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    public Button loginButton;
    public Button quitButton;

    public Text errorText;

    public InputField usernameInputField;
    public InputField passwordInputField;

    public void OnEnable()
    {
        loginButton.onClick.RemoveAllListeners();
        loginButton.onClick.AddListener(LoginButton_OnClick);

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(QuitButton_OnClick);

        usernameInputField.ActivateInputField();
        passwordInputField.ActivateInputField();

        errorText.text = "";

        if (LoginInformation.userName != null && LoginInformation.userName != "" && LoginInformation.loggedIn)
            usernameInputField.text = LoginInformation.userName;
        passwordInputField.text = "";
    }

    public void LoginButton_OnClick()
    {
        var db = DBConnection.Instance();

        var response = db.Login(new LoginMessage { UserName = usernameInputField.text, Password = passwordInputField.text });

        if (!response.isSuccessful)
            errorText.text = response.ErrorMessage;
        else
        {
            LoginInformation.userName = usernameInputField.text;
            LoginInformation.guid = new System.Guid(response.UserId);
            LoginInformation.loggedIn = false;
            MenuManager._instance.ChangePanel(MenuManager._instance.startupGui);
        }
    }

    public void QuitButton_OnClick()
    {
        Application.Quit();
    }
}
