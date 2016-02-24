using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Button saveButton;
    public Button cancelButton;

    public void OnEnable()
    {
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(SaveButton_OnClick);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(CancelButton_OnClick);
    }

    public void SaveButton_OnClick()
    {
        MenuManager._instance.ChangePanel(MenuManager._instance.startupGui);
    }

    public void CancelButton_OnClick()
    {
        MenuManager._instance.ChangePanel(MenuManager._instance.startupGui);
    }
}
