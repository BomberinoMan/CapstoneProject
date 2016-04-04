using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public InputField oldPasswordInputField;
    public InputField newPasswordInputField;
    public InputField newRePasswordInputField;

    public Button changePasswordButton;
    public Button backButton;

    public Text errorText;

    private bool _isResponse;
    private ChangePasswordResponse _response;
    private Action<ChangePasswordResponse> _responseCallback;


    public void OnEnable()
    {
        _isResponse = false;
        _responseCallback = null;

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(BackButton_OnClick);

        changePasswordButton.onClick.RemoveAllListeners();
        changePasswordButton.onClick.AddListener(ChangePasswordButton_OnClick);

        oldPasswordInputField.text = "";
        newPasswordInputField.text = "";
        newRePasswordInputField.text = "";
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

    public void ChangePasswordButton_OnClick()
    {
        if (newPasswordInputField.text != newRePasswordInputField.text)
        {
            errorText.text = "Passwords do not match";
            return;
        }
        else if (newPasswordInputField.text.Length <= 8)
        {
            errorText.text = "Password is too short";
            return;
        }

        _isResponse = false;
        _responseCallback = new Action<ChangePasswordResponse>(ChangePasswordCallback);
        new Thread(ChangePassword).Start();
    }

    public void ChangePassword()
    {
        Debug.Log(LoginInformation.username + oldPasswordInputField.text + newPasswordInputField.text);
        _response = DBConnection.instance.ChangePassword(new ChangePasswordMessage { userName = LoginInformation.username, oldPassword = oldPasswordInputField.text, newPassword = newPasswordInputField.text });
        _isResponse = true;
    }

    public void ChangePasswordCallback(ChangePasswordResponse response)
    {
        if (!response.isSuccessful)
            errorText.text = response.errorMessage;
        else
        {
            MenuManager.instance.ChangePanel(MenuManager.instance.loginGui);
        }
    }

    public void BackButton_OnClick()
    {
        MenuManager.instance.ChangePanel(MenuManager.instance.startupGui);
    }
}
