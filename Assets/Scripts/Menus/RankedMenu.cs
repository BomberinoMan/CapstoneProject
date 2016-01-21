using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RankedMenu : MonoBehaviour
{
    public void FindButton_OnClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void LogoutButton_OnClick()
    {
        SceneManager.LoadScene("StartupMenu");
    }
}
