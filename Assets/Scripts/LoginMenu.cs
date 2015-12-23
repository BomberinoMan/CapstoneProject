using UnityEngine;
using System.Collections;

public class LoginMenu : MonoBehaviour
{
    public void LoginButton_OnClick()
    {
        Application.LoadLevel("RankedMenu");
    }

    public void BackButton_OnClick()
    {
        Application.LoadLevel("StartupMenu");
    }
}
