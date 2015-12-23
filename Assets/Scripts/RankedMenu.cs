using UnityEngine;
using System.Collections;

public class RankedMenu : MonoBehaviour
{
    public void FindButton_OnClick()
    {
        Application.LoadLevel("Game");
    }

    public void LogoutButton_OnClick()
    {
        Application.LoadLevel("StartupMenu");
    }
}
