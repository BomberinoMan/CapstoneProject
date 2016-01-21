using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public void SaveButton_OnClick()
    {
        SceneManager.LoadScene("StartupMenu");
    }

    public void CancelButton_OnClick()
    {
        SceneManager.LoadScene("StartupMenu");
    }
}
