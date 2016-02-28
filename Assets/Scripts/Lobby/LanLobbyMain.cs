using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LanLobbyMain : MonoBehaviour
{
    public InputField joinIp;
    public Button hostButton;
    public Button joinButton;
    public Button backButton;

    public void OnEnable()
    {
        hostButton.onClick.RemoveAllListeners();
        hostButton.onClick.AddListener(OnClickHost);

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(OnClickJoin);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnClickBack);
    }

    public void OnClickHost()
    {
        LobbyManager._instance.StartHost();
    }

    public void OnClickJoin()
    {
        LobbyManager._instance.networkAddress = joinIp.text;
        LobbyManager._instance.StartClient();
    }

    public void OnClickBack()
    {
        Destroy(LobbyManager._instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
