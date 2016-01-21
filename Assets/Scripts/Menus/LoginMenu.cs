using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginMenu : MonoBehaviour
{
    public void LoginButton_OnClick()
    {
        SceneManager.LoadScene("RankedMenu");
    }

    public void BackButton_OnClick()
    {
        SceneManager.LoadScene("StartupMenu");
    }
}
