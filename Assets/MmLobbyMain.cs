using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MmLobbyMain : MonoBehaviour
{
    public InputField hostRoomName;
    public Button hostButton;
    public Button findButton;
    public Button backButton;

    public void OnEnable()
    {
        hostButton.onClick.RemoveAllListeners();
        hostButton.onClick.AddListener(OnClickHost);

        findButton.onClick.RemoveAllListeners();
        findButton.onClick.AddListener(OnClickFind);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnClickBack);
    }

    public void OnClickHost()
    {
        //LobbyManager._instance.StartHost();
    }

    public void OnClickFind()
    {
        //LobbyManager._instance.networkAddress = joinIp.text;
        //LobbyManager._instance.StartClient();
    }

    public void OnClickBack()
    {
        Destroy(LobbyManager._instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
