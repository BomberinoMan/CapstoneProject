using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer
{
    public GameObject scoreScreenPlayer;

    public InputField nameInput;
    public Button readyButton;
    public Button removePlayerButton;
    [SyncVar(hook = "HookNameChanged")]
    public string playerName = "";
    [SyncVar(hook = "HookReadyChanged")]
    public string readyText = "";

    public override void OnClientEnterLobby()
    {
        LobbyManager.instance.AddPlayer(this);
        LobbyPlayerList.instance.AddPlayer(this);
        SetupRemotePlayer();
    }

    public override void OnStartLocalPlayer()
    {
        SetupLocalPlayer();
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
		if (isLocalPlayer)
			return;
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
                LobbyManager.instance.StopHost();
            }
        }
        else
        {
            LobbyManager.instance.StopClient();
        }
    }

    public void OnDestroy()
    {
        LobbyManager.instance.RemovePlayer(this);
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

    [ClientRpc]
    public void RpcUpdateCountdown(int count)
    {
        LobbyManager.instance.countdownGui.gameObject.SetActive(true);
        LobbyManager.instance.countdownText.text = "Match Starting in " + (count + 1);

        if (count < 0)
        {
            LobbyManager.instance.countdownGui.gameObject.SetActive(false);
        }
    }

    public void HookNameChanged(string name)
    {
        playerName = name;
        nameInput.text = name;
    }

    [ClientRpc]             // Need to send them in two lists because of the limitations of RPC calls
    public void RpcAddPlayerToScoreList(string playerName, int playerScore)
    {
        LobbyManager.instance.scoreScreenGui.gameObject.SetActive(true);

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

        LobbyManager.instance.scoreScreenGui.gameObject.SetActive(false);
    }

    public void HookReadyChanged(string text)
    {
        readyText = text;
        readyButton.transform.GetChild(0).GetComponent<Text>().text = text;
    }
}
