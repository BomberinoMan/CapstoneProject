using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartupMenu : MonoBehaviour
{
    public void ButtonRanked_OnClick()
    {
        Debug.Log("ButtonRanked");
        SceneManager.LoadScene("LoginMenu");
    }

    public void ButtonUnranked_OnClick()
    {
        Debug.Log("ButtonUnranked");
        SceneManager.LoadScene("UnrankedMenu");
    }

    public void ButtonSettings_OnClick()
    {
        Debug.Log("ButtonSettings");
        SceneManager.LoadScene("SettingsMenu");
    }

    public void ButtonQuit_OnClick()
    {
        Debug.Log("ButtonQuit");
        Application.Quit();
    }
}
