using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartupMenu : MonoBehaviour
{
    public Button matchMakingButton;
    public Button lanButton;
    public Button settingsButton;
    public Button backButton;

    public void OnEnable()
    {
        matchMakingButton.onClick.RemoveAllListeners();
        matchMakingButton.onClick.AddListener(OnClickMatchMaking);

        lanButton.onClick.RemoveAllListeners();
        lanButton.onClick.AddListener(OnClickLan);

        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(OnClickSettings);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnClickBack);
    }

    public void OnClickMatchMaking()
    {
        SceneManager.LoadScene("MatchMakingLobby");
    }

    public void OnClickLan()
    {
        SceneManager.LoadScene("LanLobby");
    }

    public void OnClickSettings()
    {
        MenuManager.instance.ChangePanel(MenuManager.instance.settingsGui);
    }

    public void OnClickBack()
    {
        LoginInformation.username = "";
        LoginInformation.guid = System.Guid.Empty;
        LoginInformation.loggedIn = false;
        MenuManager.instance.ChangePanel(MenuManager.instance.loginGui);
    }
}
