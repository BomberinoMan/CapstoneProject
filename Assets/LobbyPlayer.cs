using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer 
{
    public InputField nameInput;
    public Button readyButton;
    public Button removePlayerButton;
    public string playerName = "";

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();
        LobbyManager._instance.AddPlayer(this);
        LobbyPlayerList._instance.AddPlayer(this);

        if (isLocalPlayer)
        {
            SetupLocalPlayer();
        }
        else
        {
            SetupRemotePlayer();
        }
    }

    public override void OnClientExitLobby()
    {
        base.OnClientExitLobby();
        LobbyManager._instance.RemovePlayer(this);
    }

    public void SetupLocalPlayer()
    {
        nameInput.interactable = true;
        readyButton.interactable = true;
        //CheckRemoveButton();
        readyButton.transform.GetChild(0).GetComponent<Text>().text = "READY UP";

        nameInput.onEndEdit.RemoveAllListeners();
        readyButton.onClick.RemoveAllListeners();

        nameInput.onEndEdit.AddListener(OnNameChanged);
        readyButton.onClick.AddListener(OnReadyClick);
    }

    public void SetupRemotePlayer()
    {
        nameInput.interactable = false;
        readyButton.interactable = false;
        //removePlayerButton.interactable = NetworkServer.active;       //setup kick button if server
        //ChangeReadyButtonColor(NotReadyColor);                        //set indicator to not ready
        readyButton.transform.GetChild(0).GetComponent<Text>().text = "Not Ready...";
        OnClientReady(false);
    }

    public void OnNameChanged(string name)
    {
        CmdNameChanged(name);
    }

    public void OnReadyClick()
    {
        SendReadyToBeginMessage();
    }

    public void OnRemovePlayerClick()
    {
        if (isLocalPlayer)
        {
            LobbyManager._instance.RemovePlayer(this);
            RemovePlayer();
        }
    }

    [Command]
    public void CmdNameChanged(string name)
    {
        playerName = name;
    }
}
