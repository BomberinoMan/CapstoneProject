using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountMenu : MonoBehaviour
{
    public InputField usernameInputField;
    public InputField passwordInputField;
    public InputField rePasswordInputField;

    public Text errorText;

    public Button createAccountButton;
    public Button backButton;

    private bool _isResponse;
    private CreateUserResponse _response;
    private Action<CreateUserResponse> _responseCallback;

    public void OnEnable()
    {
        _isResponse = false;
        _responseCallback = null;

        createAccountButton.onClick.RemoveAllListeners();
        createAccountButton.onClick.AddListener(CreateAccount_OnClick);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(Back_OnClick);

        errorText.text = "";
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

    public void CreateAccount_OnClick()
    {
        if (passwordInputField.text != rePasswordInputField.text)
        {
            errorText.text = "Passwords do not match";
            return;
        }
        else if (passwordInputField.text.Length <= 8)
        {
            errorText.text = "Password is too short";
            return;
        }

        _isResponse = false;
        _responseCallback = new Action<CreateUserResponse>(CreateUserCallback);
        new Thread(CreateUser).Start();
    }

    public void CreateUser()
    {
        _response = DBConnection.instance.CreateUser(new CreateUserMessage { userName = usernameInputField.text, password = passwordInputField.text });
        _isResponse = true;
    }

    public void CreateUserCallback(CreateUserResponse response)
    {
        if (!response.isSuccessful)
            errorText.text = response.errorMessage;
        else
            MenuManager.instance.ChangePanel(MenuManager.instance.loginGui);
    }

    public void Back_OnClick()
    {
        MenuManager.instance.ChangePanel(MenuManager.instance.loginGui);
    }
}
