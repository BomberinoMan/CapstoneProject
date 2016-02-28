using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartupMenu : MonoBehaviour
{
    public Button rankedButton;
    public Button unrankedButton;
    public Button settingsButton;
    public Button backButton;

    public void OnEnable()
    {
        rankedButton.onClick.RemoveAllListeners();
        rankedButton.onClick.AddListener(Ranked_OnClick);

        unrankedButton.onClick.RemoveAllListeners();
        unrankedButton.onClick.AddListener(Unranked_OnClick);

        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(Settings_OnClick);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(Back_OnClick);
    }

    public void Ranked_OnClick()
    {
        SceneManager.LoadScene("MatchMakingLobby");
    }

    public void Unranked_OnClick()
    {
        SceneManager.LoadScene("LanLobby");
    }

    public void Settings_OnClick()
    {
        MenuManager._instance.ChangePanel(MenuManager._instance.settingsGui);
    }

    public void Back_OnClick()
    {
        LoginInformation.userName = "";
        LoginInformation.guid = System.Guid.Empty;
        LoginInformation.loggedIn = false;
        MenuManager._instance.ChangePanel(MenuManager._instance.loginGui);
    }
}
