using UnityEngine;
using System.Collections;

public class StartupMenu : MonoBehaviour
{
    public void ButtonRanked_OnClick()
    {
        Debug.Log("ButtonRanked");
        Application.LoadLevel("LoginMenu");
    }

    public void ButtonUnranked_OnClick()
    {
        Debug.Log("ButtonUnranked");
        Application.LoadLevel("UnrankedMenu");
    }

    public void ButtonSettings_OnClick()
    {
        Debug.Log("ButtonSettings");
        Application.LoadLevel("SettingsMenu");
    }

    public void ButtonQuit_OnClick()
    {
        Debug.Log("ButtonQuit");
        Application.Quit();
    }
}
