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

        SetupRemotePlayer();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        SetupLocalPlayer();
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
        removePlayerButton.interactable = true;

        readyButton.transform.GetChild(0).GetComponent<Text>().text = "READY UP";
        removePlayerButton.transform.GetChild(0).GetComponent<Text>().text = "QUIT";

        nameInput.onEndEdit.RemoveAllListeners();
        nameInput.onEndEdit.AddListener(OnNameChanged);
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClick);
        removePlayerButton.onClick.RemoveAllListeners();
        removePlayerButton.onClick.AddListener(OnRemovePlayerClick);
    }

    public void SetupRemotePlayer()
    {
        nameInput.interactable = false;
        readyButton.interactable = false;
        readyButton.transform.GetChild(0).GetComponent<Text>().text = "Not Ready...";

        if (isServer)
        {
            removePlayerButton.interactable = true;
            removePlayerButton.transform.GetChild(0).GetComponent<Text>().text = "KICK";
            removePlayerButton.onClick.RemoveAllListeners();
            removePlayerButton.onClick.AddListener(OnRemovePlayerClick);
        }
        else
        {
            removePlayerButton.interactable = false;
        }
    }

    public void OnNameChanged(string name)
    {
        CmdNameChanged(name);
    }

    public void OnReadyClick()
    {
        if (!readyToBegin)
        {
            SendReadyToBeginMessage();
            readyButton.transform.GetChild(0).GetComponent<Text>().text = "READY";
        }
        else
        {
            SendNotReadyToBeginMessage();
            readyButton.transform.GetChild(0).GetComponent<Text>().text = "READY UP";
        }
    }

    public void OnRemovePlayerClick()
    {
        if (isServer)
        {
            connectionToClient.Disconnect();
            if (isLocalPlayer)
            {
                LobbyManager._instance.StopHost();
            }
        }
        else
        {
            LobbyManager._instance.StopClient();
        }
    }

    public void OnDestroy()
    {
        Debug.Log("PlayerDestroy");
        LobbyManager._instance.RemovePlayer(this);
    }

    [Command]
    public void CmdNameChanged(string name)
    {
        playerName = name;
    }

    [ClientRpc]
    public void RpcUpdateCountdown(int countdown)
    {
        //LobbyManager._instance.countdownPanel.UIText.text = "Match Starting in " + RpcUpdateCountdown;
        //LobbyManager._instance.countdownPanel.gameObject.SetActive(true);

        //if (countdown == 0)
        //{
        //    LobbyManager._instance.countdownPanel.gameObject.SetActive(false);
        //}
    }
}
