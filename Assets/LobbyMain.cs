using UnityEngine;
using System.Collections;

public class LobbyMain : MonoBehaviour
{
    public LobbyManager lobbyManager;

    public void OnClickHost()
    {
        lobbyManager.StartHost();
    }

    public void OnClickJoin()
    {
        lobbyManager.StartClient();
    }
}
