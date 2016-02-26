using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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
		LobbyManager._instance.AddPlayer(this);
        LobbyPlayerList._instance.AddPlayer(this);
        SetupRemotePlayer();
    }

    public override void OnStartLocalPlayer()
    {
        SetupLocalPlayer();
    }

    public override void OnClientExitLobby()
    {	//TODO evan
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
        LobbyManager._instance.countdownGui.gameObject.SetActive(true);
        LobbyManager._instance.countdownText.text = "Match Starting in " + (count + 1);

        if (count < 0)
        {
            LobbyManager._instance.countdownGui.gameObject.SetActive(false);
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
		LobbyManager._instance.scoreScreenGui.gameObject.SetActive(true);

        var playerRow = Instantiate(scoreScreenPlayer);
        playerRow.GetComponentInChildren<ScoreScreenPlayerName>().SetPlayerName(playerName);
        playerRow.GetComponentInChildren<ScoreScreenPlayerScore>().SetPlayerScore(playerScore);

		playerRow.transform.SetParent(LobbyManager._instance.scoreScreenGui.transform);
    }

	[ClientRpc]             // Need to send them in two lists because of the limitations of RPC calls
	public void RpcClearScoreList()
	{
		for (int i = 0; i < LobbyManager._instance.scoreScreenGui.childCount; i++)
			Destroy (LobbyManager._instance.scoreScreenGui.GetChild (i).gameObject);
		
		LobbyManager._instance.scoreScreenGui.gameObject.SetActive(false);
	}

    public void HookReadyChanged(string text)
    {
        readyText = text;
        readyButton.transform.GetChild(0).GetComponent<Text>().text = text;
    }
}
