using UnityEngine;
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
        var db = DBConnection.GetInstance();

        var response = db.Login(new LoginMessage { userName = usernameInputField.text, password = passwordInputField.text });

        if (!response.isSuccessful)
            errorText.text = response.errorMessage;
        else
        {
            LoginInformation.userName = usernameInputField.text;
            LoginInformation.guid = new System.Guid(response.userId);
            LoginInformation.loggedIn = false;
            MenuManager.instance.ChangePanel(MenuManager.instance.startupGui);
        }
    }

    public void QuitButton_OnClick()
    {
        Application.Quit();
    }
}
