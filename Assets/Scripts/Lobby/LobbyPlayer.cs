using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer
{
    public GameObject playerScoreInfo;
    public Text nameText;
    public Button readyButton;
    public Button leaveButton;

    public long nodeId;

    [SyncVar]
    private string readyText;
    [SyncVar]
    private string username;
    [SyncVar]
    public int score;
    [SyncVar]
    public bool isAlive;

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();
    }

    public override void OnClientExitLobby()
    {
        base.OnClientExitLobby();
        LobbyManager.instance.HideScorePanel();
        LobbyManager.instance.HideInGameMenu();
        LobbyManager.instance.HideInfoPanel();
    }

    public override void OnStartLocalPlayer()
    {
        UpdateUsernameLocal(LoginInformation.username);
        CmdUpdateUsernameServer(LoginInformation.username);

        LobbyManager.instance.localPlayer = this;
        if (LobbyManager.instance.isMatchMaking)
        {
            nodeId = (long)LobbyManager.instance.matchInfo.nodeId;
        }

        CmdUpdateAllPlayerLists();
    }

    public void ToggleReady()
    {
        if (!readyToBegin)
        {
            Ready();
        }
        else
        {
            NotReady();
        }
    }

    private void Ready()
    {
        SendReadyToBeginMessage();
        CmdUpdateReadyText();
    }

    private void NotReady()
    {
        SendNotReadyToBeginMessage();
        CmdUpdateReadyText();
    }

    public void Leave()
    {
        LobbyManager.instance.DisplayInfoNotification("Quitting...");
        if (LobbyManager.instance.isMatchMaking)
        {
            LeaveMatchMaking();
        }
        else
        {
            LeaveLan();
        }
    }

    private void LeaveLan()
    {
		if (isServer)
		{
			DBConnection.instance.DeleteRoom (new DeleteRoomMessage { userId = LoginInformation.guid });
			LobbyManager.instance.StopHost();
		}
        else
        {
            RemovePlayer();
            LobbyManager.instance.StopClient();
        }

    }

    private void LeaveMatchMaking()
    {
        if (isServer)
        {
            var id = LobbyManager.instance.matchInfo.networkId;
            LobbyManager.instance.matchMaker.DestroyMatch(id, OnMatchDestroyed);
        }
        else
        {
            var request = new DropConnectionRequest();
            request.networkId = LobbyManager.instance.matchInfo.networkId;
            request.nodeId = LobbyManager.instance.matchInfo.nodeId;
            LobbyManager.instance.matchMaker.DropConnection(request, OnConnectionDropped);
        }
    }

    private void OnMatchDestroyed(BasicResponse response)
    {
        LobbyManager.instance.StopHost();
    }

    private void OnConnectionDropped(BasicResponse response)
    {
        //Unity Matchmaking magic
    }

    [Command] 
    public void CmdUpdateUsernameServer(string name)
    {
        username = name;
    }

    //[Command]
    //public void CmdUpdateUsername()
    //{
    //    foreach (LobbyPlayer player in LobbyManager.instance.lobbySlots)
    //    {
    //        if (player != null)
    //        {
    //        }
    //    }
    //}

    [Command]
    public void CmdUpdateReadyText()
    {
        foreach (LobbyPlayer player in LobbyManager.instance.lobbySlots)
        {
            if (player != null)
            {
                player.RpcUpdateReadyTextRemote();
            }
        }
    }
    
    [Command]
    public void CmdUpdateAllPlayerLists()
    {
        LobbyManager.instance.UpdateAllPlayerLists();
    }

    [ClientRpc]
    public void RpcCancelCountdown()
    {
        leaveButton.interactable = true;
        readyButton.interactable = true;
        LobbyManager.instance.HideInfoPanel();
    }

    [ClientRpc]
    public void RpcUpdateCountdown(int count)
    {
        LobbyManager.instance.infoGui.gameObject.SetActive(true);
        LobbyManager.instance.infoButton.gameObject.SetActive(false);
        leaveButton.interactable = false;
        readyButton.interactable = false;

        LobbyManager.instance.infoText.text = "Match Starting in " + (count);

        if (count <= -1)
        {
            LobbyManager.instance.HideInfoPanel();
        }
    }

    [ClientRpc]
    public void RpcShowScoreList()
    {
        LobbyManager.instance.ShowScorePanel();
    }

    [ClientRpc]
    public void RpcHideScoreList()
    {
        LobbyManager.instance.HideScorePanel();
    }

    [ClientRpc]
    public void RpcUpdatePlayerList()
    {
        LobbyManager.instance.lobbyRoom.UpdatePlayerList();
    }

    [ClientRpc]
    public void RpcUpdateReadyTextRemote()
    {
        foreach (LobbyPlayer player in LobbyManager.instance.lobbySlots)
        {
            if (player != null)
            {
                //update each players text ***NOTE THIS IS THE CLIENTS VERSION OF ALL THE PLAYERS, NOT EACH INDIVIUAL PLAYER
                player.UpdateReadyTextLocal();
            }
        }
    }

    [ClientRpc]
    public void RpcClearReadyStatusLocal()
    {
        Debug.Log("CLIENT RESET");
        foreach (LobbyPlayer player in LobbyManager.instance.lobbySlots)
        {
            if (player != null)
            {
                player.readyToBegin = false;
                player.readyButton.transform.GetChild(0).GetComponent<Text>().text = "NOT READY";
            }
        }
    }

    public void UpdateReadyTextLocal()
    {
        if (readyToBegin)
        {
            readyButton.transform.GetChild(0).GetComponent<Text>().text = "READY";
        }
        else
        {
            readyButton.transform.GetChild(0).GetComponent<Text>().text = "NOT READY";
        }
    }

    //public void OnReadyChanged(string text)
    //{
    //    readyButton.transform.GetChild(0).GetComponent<Text>().text = text;
    //}

    //public void OnNameChanged(string text)
    //{
    //    nameText.text = text;
    //}

    public void UpdateUsernameLocal(string name)
    {
        username = name;
    }

    public string GetUsername()
    {
        return username;
    }

    public int GetPosition()
    {
        var position = 0;
        foreach (LobbyPlayer player in LobbyManager.instance.lobbySlots)
        {
            if (player != null)
            {
                if (this == player)
                {
                    return position;
                }
            }
            position += 1;
        }
        return -1;
    }
}
