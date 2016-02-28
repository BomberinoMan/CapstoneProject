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

        var db = DBConnection.Instance();

        var response = db.ChangePassword(new ChangePasswordMessage { UserName = LoginInformation.userName, OldPassword = oldPasswordInputField.text, NewPassword = newPasswordInputField.text });

        if (!response.isSuccessful)
            errorText.text = response.ErrorMessage;
        else
        {
            MenuManager._instance.ChangePanel(MenuManager._instance.loginGui);
        }
    }

    public void BackButton_OnClick()
    {
        MenuManager._instance.ChangePanel(MenuManager._instance.startupGui);
    }
}
