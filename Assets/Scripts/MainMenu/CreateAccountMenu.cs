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

    public void OnEnable()
    {
        createAccountButton.onClick.RemoveAllListeners();
        createAccountButton.onClick.AddListener(CreateAccount_OnClick);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(Back_OnClick);

        errorText.text = "";

        usernameInputField.ActivateInputField();
        passwordInputField.ActivateInputField();
        rePasswordInputField.ActivateInputField();
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

        var db = DBConnection.GetInstance();

        var response = db.CreateUser(new CreateUserMessage { userName = usernameInputField.text, password = passwordInputField.text });

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
