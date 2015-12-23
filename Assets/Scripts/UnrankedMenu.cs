using UnityEngine;
using System.Collections;

public class UnrankedMenu : MonoBehaviour
{
    public void SetUsername()
    {

    }

    public void JoinButton_OnClick()
    {
        Application.LoadLevel("Game");
    }

    public void CreateButton_OnClick()
    {
        Application.LoadLevel("Game");
    }

    public void BackButton_OnClick()
    {
        Application.LoadLevel("StartupMenu");
    }
}
