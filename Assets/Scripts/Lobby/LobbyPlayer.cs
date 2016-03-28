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

    [SyncVar(hook = "OnReadyChanged")]
    private string readyText;
    [SyncVar(hook = "OnNameChanged")]
    private string username;
    [SyncVar]
    public int score;
    [SyncVar]
    public bool isAlive;

    public void OnEnable()
    {
        leaveButton = LobbyRoom.instance.leaveButton;
    }

    public override void OnClientEnterLobby()
    {
        LobbyRoom.instance.AddPlayer(this);
        SetupRemotePlayer();
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
        SetupLocalPlayer();
    }

    public void SetupLocalPlayer()
    {
        CmdNameChanged(LoginInformation.username);
        CmdReadyChanged("NOT READY");
        LobbyManager.instance.localPlayer = this;

        if (LobbyManager.instance.isMatchMaking)
        {
            nodeId = (long)LobbyManager.instance.matchInfo.nodeId;
        }

        readyButton.interactable = true;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClick);

        leaveButton.interactable = true;
        leaveButton.GetComponentInChildren<Text>().text = "Leave Room";
        leaveButton.onClick.RemoveAllListeners();
        leaveButton.onClick.AddListener(OnClickLeave);
    }

    public void SetupRemotePlayer()
    {
        if (isLocalPlayer)
        {
            return;
        }

        nameText.text = username;
        readyButton.interactable = false;
        readyButton.transform.GetChild(0).GetComponent<Text>().text = readyText;
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

    public void OnClickLeave()
    {
        Leave();
    }

    public void Leave()
    {
        LobbyManager.instance.DisplayInfoNotification("Quitting...");
        if (LobbyManager.instance.isMatchMaking)
        {
            Debug.Log("LEAVE MM");
            LeaveMatchMaking();
        }
        else
        {
            Debug.Log("LEAVE LAN");
            LeaveLan();
        }
    }

    public void LeaveLan()
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

    public void LeaveMatchMaking()
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

    }

    [Command]
    public void CmdNameChanged(string name)
    {
        nameText.text = name;
        username = name;
    }

    [Command]
    public void CmdReadyChanged(string newReadyText)
    {
        readyText = newReadyText;
        readyButton.transform.GetChild(0).GetComponent<Text>().text = readyText;
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
    public void RpcResetReadyState()
    {
        CmdReadyChanged("NOT READY");
        readyButton.transform.GetChild(0).GetComponent<Text>().text = "NOT READY";
    }

    public void OnReadyChanged(string text)
    {
        readyButton.transform.GetChild(0).GetComponent<Text>().text = text;
    }

    public void OnNameChanged(string text)
    {
        nameText.text = text;
    }

    public string GetUsername()
    {
        return username;
    }
}
