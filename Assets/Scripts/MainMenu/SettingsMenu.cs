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

    public void OnEnable()
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(BackButton_OnClick);

        changePasswordButton.onClick.RemoveAllListeners();
        changePasswordButton.onClick.AddListener(ChangePasswordButton_OnClick);

        oldPasswordInputField.text = "";
        newPasswordInputField.text = "";
        newRePasswordInputField.text = "";
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

        var response = DBConnection.instance.ChangePassword(new ChangePasswordMessage { userName = LoginInformation.username, oldPassword = oldPasswordInputField.text, newPassword = newPasswordInputField.text });

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
