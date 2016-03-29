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
    public Image playerColorImage;

    private Color[] playerSlotColors = new Color[]
    {
        new Color(0.0f, 0.0f, 1.0f, 1.0f),  // Blue
        new Color(1.0f, 1.0f, 0.0f, 1.0f),  // Yellow
        new Color(1.0f, 0.0f, 0.0f, 1.0f),  // Red
        new Color(0.0f, 1.0f, 0.0f, 1.0f)   // Green
    };

    public long nodeId;

    [SyncVar(hook = "OnReadyChanged")]
    public string readyText;
    [SyncVar(hook = "OnNameChanged")]
    private string username;
    [SyncVar]
    public int score;
    [SyncVar]
    public bool isAlive;

    public override void OnClientEnterLobby()
    {
        LobbyRoom.instance.AddPlayer(this);
        playerColorImage.color = playerSlotColors[slot];
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
        LobbyManager.instance.localPlayer = this;

        CmdSetUsername(LoginInformation.username);
        CmdUpdateReadyStateText();

        readyButton.interactable = true;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClick);

		leaveButton = LobbyRoom.instance.leaveButton;
        leaveButton.interactable = true;
        leaveButton.GetComponentInChildren<Text>().text = "Leave Room";
        leaveButton.onClick.RemoveAllListeners();
        leaveButton.onClick.AddListener(OnClickLeave);
    }

    public void SetupRemotePlayer()
    {
		if (isLocalPlayer)
			return;

        nameText.text = username;
        readyButton.interactable = false;
        readyButton.transform.GetChild(0).GetComponent<Text>().text = readyText;
    }

    public void OnReadyClick()
    {
        if (!readyToBegin)
            SendReadyToBeginMessage();
        else
            SendNotReadyToBeginMessage();

		CmdUpdateReadyStateText();
    }

    public void OnClickLeave()
    {
        Leave();
    }

    public void Leave()
    {
        LobbyManager.instance.DisplayInfoNotification("Leaving");
        if (LobbyManager.instance.isMatchMaking)
        {
            LeaveMatchMaking();
        }
        else
        {
            LeaveLan();
        }
    }

    public void LeaveLan()
    {
		if (isServer)
		{
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
    public void CmdSetUsername(string name)
    {
        username = name;
    }

    [Command]
	public void CmdUpdateReadyStateText()
    {
		readyText = readyToBegin ? "READY" : "NOT READY";
    }

    [ClientRpc]
    public void RpcCancelCountdown()
    {
		if (!isLocalPlayer)
			return;
		
        leaveButton.interactable = true;
        readyButton.interactable = true;
        LobbyManager.instance.HideInfoPanel();
    }

    [ClientRpc]
    public void RpcUpdateCountdown(int count)
    {
		if (!isLocalPlayer)
			return;
		
        LobbyManager.instance.infoGui.gameObject.SetActive(true);
        LobbyManager.instance.infoButton.gameObject.SetActive(false);
        leaveButton.interactable = false;
        readyButton.interactable = false;

        LobbyManager.instance.infoText.text = "Match Starting in " + (count);

        if (count <= 0)
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

    public void OnReadyChanged(string text)
    {
		readyText = text;
		readyButton.transform.GetChild(0).GetComponent<Text>().text = readyText;
    }

    public void OnNameChanged(string text)
    {
        nameText.text = text;
		username = text;
    }

    public string GetUsername()
    {
        return username;
    }
}
