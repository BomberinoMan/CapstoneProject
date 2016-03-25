using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class LanLobbyMain : MonoBehaviour
{
    public InputField joinIp;
    public Button hostButton;
    public Button joinButton;
    public Button backButton;

    public void OnEnable()
    {
        /*
        var response = DBConnection.instance.CreateRooom(new CreateRoomMessage { userId = new Guid(), name = "Second test room" });

        var response2 = DBConnection.instance.ListRooms(new ListRoomsMessage { userId = new Guid() });

        var response3 = DBConnection.instance.DeleteRoom(new DeleteRoomMessage { userId = new Guid() });
        */
        hostButton.onClick.RemoveAllListeners();
        hostButton.onClick.AddListener(OnClickHost);

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(OnClickJoin);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnClickBack);
    }

    public void OnClickHost()
    {
        LobbyManager.instance.StartHost();
    }

    public void OnClickJoin()
    {
        LobbyManager.instance.networkAddress = joinIp.text;
        LobbyManager.instance.StartClient();
    }

    public void OnClickBack()
    {
        Destroy(LobbyManager.instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
