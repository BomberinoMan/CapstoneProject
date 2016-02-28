using UnityEngine;
using UnityEngine.SceneManagement;

public class UnrankedMenu : MonoBehaviour
{
    public void SetUsername()
    {
        // DO nothing
    }

    public void JoinButton_OnClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void CreateButton_OnClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void BackButton_OnClick()
    {
        SceneManager.LoadScene("StartupMenu");
    }
}
