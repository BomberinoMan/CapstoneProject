using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer
{
    public InputField nameInput;
    public Button readyButton;
    public Button removePlayerButton;
    [SyncVar(hook = "HookNameChanged")]
    public string playerName = "";
    [SyncVar(hook = "HookReadyChanged")]
    public string readyText = "";

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

        readyText = "NOT READY";
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
        readyButton.transform.GetChild(0).GetComponent<Text>().text = "NOT READY";
        removePlayerButton.transform.GetChild(0).GetComponent<Text>().text = "QUIT";

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
            CmdReadyChanged("READY");
        }
        else
        {
            SendNotReadyToBeginMessage();
            CmdReadyChanged("NOT READY");
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
        LobbyManager._instance.RemovePlayer(this);
    }

    [Command]
    public void CmdNameChanged(string name)
    {
        playerName = name;
    }

    [Command]
    public void CmdReadyChanged(string newReadyText)
    {
        readyText = newReadyText;
    }

    //[ClientRpc]
    //public void RpcUpdateReady(string newReadyText)
    //{
    //    readyText = newReadyText;
    //    readyButton.transform.GetChild(0).GetComponent<Text>().text = newReadyText;
    //}

    //[ClientRpc]
    //public void RpcUpdateName(string name)
    //{
    //    playerName = name;
    //    nameInput.text = playerName;
    //}

    [ClientRpc]
    public void RpcUpdateCountdown(int count)
    {
        LobbyManager._instance.countdownText.text = "Match Starting in " + (count + 1);
    }

    public void HookNameChanged(string name)
    {
        playerName = name;
        nameInput.text = name;
    }

    public void HookReadyChanged(string text)
    {
        readyText = text;
        readyButton.transform.GetChild(0).GetComponent<Text>().text = text;
    }
}
