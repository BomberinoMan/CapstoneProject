using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer 
{
    public RectTransform playerPanel;
    public Text readyText;
    public LobbyManager lobbyManager;

    private bool isReady = false;

    public void OnEnable()
    {
        transform.parent = lobbyManager.lobbyPanel.transform;
    }

    public void OnReadyClick()
    {
        //if (!isLocalPlayer)
        //{
        //    return;
        //}

        if (!isReady)
        {
            isReady = true;
            readyText.text = "Ready";
            SendReadyToBeginMessage();
        }
        else
        {
            isReady = false;
            readyText.text = "Ready Up";
            SendNotReadyToBeginMessage();
        }
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();
        lobbyManager.AddPlayer(this);
    }

    public override void OnClientExitLobby()
    {
        base.OnClientExitLobby();
        lobbyManager.RemovePlayer(this);
    }
}
