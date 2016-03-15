using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer
{
    public GameObject scoreScreenPlayer;
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

    public override void OnStartLocalPlayer()
    {
        SetupLocalPlayer();
    }

    public void SetupLocalPlayer()
    {
        CmdNameChanged(LoginInformation.username);
        CmdReadyChanged("NOT READY");

        nodeId = (long)LobbyManager.instance.matchInfo.nodeId;

        readyButton.interactable = true;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClick);

        leaveButton.interactable = true;
        leaveButton.transform.GetChild(0).GetComponent<Text>().text = "Leave Room";
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
        LobbyManager.instance.DisplayInfoNotification("Quitting...");
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
    }

    [ClientRpc]
    public void RpcCancelCountdown()
    {
        leaveButton.interactable = true;
        readyButton.interactable = true;
		LobbyManager.instance.HideInfoPanel ();
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
			LobbyManager.instance.HideInfoPanel ();
        }
    }

    [ClientRpc]
    public void RpcAddPlayerToScoreList(string playerName, int playerScore)
    {
		LobbyManager.instance.ChangePanel (LobbyManager.instance.scoreScreenGui);

        var playerRow = Instantiate(scoreScreenPlayer);
        playerRow.GetComponentInChildren<ScoreScreenPlayerName>().SetPlayerName(playerName);
        playerRow.GetComponentInChildren<ScoreScreenPlayerScore>().SetPlayerScore(playerScore);

		playerRow.transform.SetParent(LobbyManager.instance.scoreScreenGui.transform);
    }

    [ClientRpc]             // Need to send them in two lists because of the limitations of RPC calls
    public void RpcClearScoreList()
    {
		for (int i = 0; i < LobbyManager.instance.scoreScreenGui.childCount; i++)
			Destroy(LobbyManager.instance.scoreScreenGui.GetChild(i).gameObject);

		LobbyManager.instance.scoreScreenGui.gameObject.SetActive (false);
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
