using UnityEngine;

public class MenuManager : MonoBehaviour
{
    static public MenuManager instance;

    public LoginMenu loginMenu;
    public StartupMenu startupMenu;
    public SettingsMenu settingsMenu;
    public CreateAccountMenu accountMenu;

    public RectTransform loginGui;
    public RectTransform startupGui;
    public RectTransform settingsGui;
    public RectTransform accountgui;

    private RectTransform _currentPanel;

    void Start()
    {
        instance = this;
        
        if (LoginInformation.loggedIn)
            ChangePanel(startupGui);
        else
            ChangePanel(loginGui);
    }

    public void ChangePanel(RectTransform newPanel)
    {
		if (_currentPanel != null)
			_currentPanel.gameObject.SetActive (false);
		else 
		{
			loginGui.gameObject.SetActive (false);
			startupGui.gameObject.SetActive (false);
			accountgui.gameObject.SetActive (false);
			settingsGui.gameObject.SetActive (false);
		}

        if (newPanel != null)
            newPanel.gameObject.SetActive(true);

        _currentPanel = newPanel;
    }
}
