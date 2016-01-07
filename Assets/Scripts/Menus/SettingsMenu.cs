using UnityEngine;
using System.Collections;

public class SettingsMenu : MonoBehaviour
{
    public void SaveButton_OnClick()
    {
        Application.LoadLevel("StartupMenu");
    }

    public void CancelButton_OnClick()
    {
        Application.LoadLevel("StartupMenu");
    }
}
