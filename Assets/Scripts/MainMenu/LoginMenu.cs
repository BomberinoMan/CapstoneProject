using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

public class LoginMenu : MonoBehaviour
{
    public Button loginButton;
    public Button quitButton;
    public Button createAccountButton;
	public Button guestButton;

    public Text errorText;

    public InputField usernameInputField;
    public InputField passwordInputField;

    private bool _isResponse;
    private LoginResponse _response;
    private Action<LoginResponse> _responseCallback;

    public void OnEnable()
    {
        _isResponse = false;
        _responseCallback = null;

        loginButton.onClick.RemoveAllListeners();
        loginButton.onClick.AddListener(LoginButton_OnClick);

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(QuitButton_OnClick);

        createAccountButton.onClick.RemoveAllListeners();
        createAccountButton.onClick.AddListener(CreateAccount_OnClick);

		guestButton.onClick.RemoveAllListeners ();
		guestButton.onClick.AddListener (GuestButton_OnClick);

        errorText.text = "";
		passwordInputField.text = "";

        if (LoginInformation.username != null && LoginInformation.username != "" && LoginInformation.loggedIn)
            usernameInputField.text = LoginInformation.username;
    }

    void Update()
    {
        if (_isResponse)
        {
            if (_responseCallback != null)
            {
                _responseCallback(_response);
            }
            _isResponse = false;
        }
    }

    public void LoginButton_OnClick()
    {
        _isResponse = false;
        _responseCallback = new Action<LoginResponse>(LoginCallback);
        new Thread(Login).Start();
    }

    public void Login()
    {
        _response = DBConnection.instance.Login(new LoginMessage { userName = usernameInputField.text, password = passwordInputField.text });
        _isResponse = true;
    }

    public void LoginCallback(LoginResponse response)
    {
        if (!response.isSuccessful)
            errorText.text = response.errorMessage;
        else
        {
            LoginInformation.username = usernameInputField.text;
            LoginInformation.guid = new Guid(response.userId);
            LoginInformation.loggedIn = true;
            MenuManager.instance.ChangePanel(MenuManager.instance.startupGui);
        }
    }

	public void GuestButton_OnClick()
	{
		LoginInformation.username = "Guest";
		LoginInformation.guid = Guid.Empty;
		LoginInformation.loggedIn = true;

		MenuManager.instance.ChangePanel (MenuManager.instance.startupGui);
	}

    public void CreateAccount_OnClick()
    {
        MenuManager.instance.ChangePanel(MenuManager.instance.accountgui);
    }

    public void QuitButton_OnClick()
    {
        Application.Quit();
    }
}
