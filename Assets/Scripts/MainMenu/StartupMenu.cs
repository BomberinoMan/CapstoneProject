using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartupMenu : MonoBehaviour
{
    public Button rankedButton;
    public Button unrankedButton;
    public Button settingsButton;
    public Button quitButton;

    public void OnEnable()
    {
        rankedButton.onClick.RemoveAllListeners();
        rankedButton.onClick.AddListener(Ranked_OnClick);

        unrankedButton.onClick.RemoveAllListeners();
        unrankedButton.onClick.AddListener(Unranked_OnClick);

        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(Settings_OnClick);

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(Quit_OnClick);
    }

    public void Ranked_OnClick()
    {
        MenuManager._instance.ChangePanel(MenuManager._instance.loginGui);
    }

    public void Unranked_OnClick()
    {
        SceneManager.LoadScene("LanLobby");
    }

    public void Settings_OnClick()
    {
        MenuManager._instance.ChangePanel(MenuManager._instance.settingsGui);
    }

    public void Quit_OnClick()
    {
        Application.Quit();
    }
}
