using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer
{
    public GameObject scoreScreenPlayer;

    public Text nameText;
    public Button readyButton;
    public Button removePlayerButton;

    [SyncVar(hook = "HookReadyChanged")]
    public string readyText = "";
    [SyncVar]
    public string username;

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
        CmdNameChanged(LoginInformation.userName);
        
        readyButton.interactable = true;
        readyText = "NOT READY";
        readyButton.transform.GetChild(0).GetComponent<Text>().text = readyText;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnReadyClick);
        
        removePlayerButton.interactable = true;
        removePlayerButton.transform.GetChild(0).GetComponent<Text>().text = "QUIT";
        removePlayerButton.onClick.RemoveAllListeners();
        removePlayerButton.onClick.AddListener(OnRemovePlayerClick);
    }

    public void SetupRemotePlayer()
    {
        if (isLocalPlayer)
            return;

        nameText.text = username;

        readyButton.interactable = false;
        readyButton.transform.GetChild(0).GetComponent<Text>().text = readyText;
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
                var id = LobbyManager.instance.matchInfo.networkId;
                LobbyManager.instance.matchMaker.DestroyMatch(id, OnMatchDestroyed);
                LobbyManager.instance.StopHost();
            }
        }
        else
        {
            LobbyManager.instance.StopClient();
        }
    }

    private void OnMatchDestroyed(BasicResponse response)
    {
        Debug.Log("MATCH DESTROYED: " + response.ToString());
    }

    public void OnDestroy()
    {
        LobbyManager.instance.RemovePlayer(this);
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
        removePlayerButton.interactable = true;
        readyButton.interactable = true;
        LobbyManager.instance.infoGui.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcUpdateCountdown(int count)
    {
        LobbyManager.instance.infoGui.gameObject.SetActive(true);
        LobbyManager.instance.infoButton.gameObject.SetActive(false);
        removePlayerButton.interactable = false;
        readyButton.interactable = false;

        LobbyManager.instance.infoText.text = "Match Starting in " + (count);

        if (count <= -1)
        {
            LobbyManager.instance.infoGui.gameObject.SetActive(false);
        }
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
