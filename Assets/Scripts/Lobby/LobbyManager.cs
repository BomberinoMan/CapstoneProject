using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class LobbyManager : NetworkLobbyManager
{
    public GameObject gameManager;
    public static LobbyManager instance;
    public ScoreScreen scoreScreen;
    public LobbyRoom lobbyRoom;

    public RectTransform scoreScreenGui;
    public RectTransform lobbyGui;
    public RectTransform menuGui;
    public RectTransform infoGui;
    public RectTransform inGameMenu;
    public Text infoText;
    public Button infoButton;
    public float countdownTime = 5.0f;

    public bool isMatchMaking = false;
    public LobbyPlayer localPlayer;

    private RectTransform _currentPanel;
    private bool _sceneLoaded = false;
    private GameObject[] playerGameObjects = new GameObject[4];

    void Start()
    {
        instance = this;

        // Activate needed objects to play the game
        gameManager.SetActive(true);
        ChangePanel(menuGui);
    }

    public void GameIsOver()
    {
        _sceneLoaded = false;
        ServerReturnToLobby();
    }

    // **************SERVER**************
    public override void OnLobbyStopHost()
    {
        base.OnLobbyStopHost();

        //TODO destroy match if possible
        // maybe create private var for matchId
        // or blacklist match with that id
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        UpdateAllPlayerLists();
    }

    public void UpdateAllPlayerLists()
    {
        foreach (LobbyPlayer player in LobbyManager.instance.lobbySlots)
        {
            if (player != null)
            {
                player.RpcUpdatePlayerList();
            }
        }
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection networkConnection, short playerControllerId)
    {
        // Figure out what slot the player is in based on the network connection and playerControllerId
        var i = lobbySlots.Where(x => x != null && x.connectionToClient.connectionId == networkConnection.connectionId && x.playerControllerId == playerControllerId).First().slot;

        GameObject newPlayer = (GameObject)Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
        newPlayer.transform.position = GameManager.instance.playerSpawnVectors[i];
        newPlayer.GetComponent<PlayerControllerComponent>().slot = (int)i;

        playerGameObjects[i] = newPlayer;

        NetworkServer.Spawn(newPlayer);
        return newPlayer;
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject gameObject1, GameObject gameObject2)
    {
        if (!_sceneLoaded)
            GameManager.instance.SpawnBoard();
        _sceneLoaded = true;
        return true;
    }

    public override void OnLobbyServerPlayersReady()
    {
        if (ArePlayersReady())
        {
            StartCoroutine(CountDownCoroutine());
        }
    }

    public IEnumerator CountDownCoroutine()
    {
        float remainingTime = countdownTime + 1;
        int floorTime = Mathf.FloorToInt(remainingTime);
        int playerCount = lobbySlots.Where(x => x != null).Count();

        while (remainingTime >= 0)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime);

            if (!ArePlayersReady() || playerCount != lobbySlots.Where(x => x != null).Count())
            {
                foreach (LobbyPlayer player in lobbySlots)
                {
                    if (player != null)
                    {
                        player.RpcCancelCountdown();
                    }
                }
                yield break;
            }

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;

                foreach (LobbyPlayer player in lobbySlots)
                {
                    if (player != null)
                    {
                        player.RpcUpdateCountdown(floorTime);
                    }
                }
            }
        }

        for (int i = 0; i < lobbySlots.Count(); i++)
            if (lobbySlots[i] != null)
                (lobbySlots[i] as LobbyPlayer).isAlive = true;

        Debug.Log("STATUS RESET");
        foreach (LobbyPlayer player in lobbySlots)
        {
            if (player != null)
            {
                player.RpcClearReadyStatusLocal();
            }
        }

        ServerChangeScene(playScene);
    }

    public bool ArePlayersReady()
    {
        foreach (LobbyPlayer player in lobbySlots)
        {
            if (player != null)
            {
                if (!player.readyToBegin)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public GameObject GetPlayerGameObject(int index)
    {
        if (index < 0 || index >= 4)
            return null;

        return playerGameObjects[index];
    }

    public void KickPlayer(NetworkConnection conn)
    {
        conn.Disconnect();
    }

    // **************CLIENT**************
    public override void OnLobbyClientExit()
    {
        ChangePanel(menuGui);
        HideInfoPanel();
    }

    public override void OnLobbyClientEnter()
    {
        ChangePanel(lobbyGui);
        HideInfoPanel();
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.LogError("CLIENT ERROR" + errorCode);
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == "Game")
            DisableAllPanels();
        //TODO need to enable lobby gui here too?
    }

    // **************GUI**************
    public void DisplayInfoNotification(string message)
    {
        infoText.text = message;
        infoButton.gameObject.SetActive(false);
        infoGui.gameObject.SetActive(true);
    }

    public void DisplayInfoAlert(string message, UnityAction onCancel)
    {
        infoText.text = message;
        infoButton.onClick.RemoveAllListeners();
        infoButton.onClick.AddListener(onCancel);

        infoButton.gameObject.SetActive(true);
        infoGui.gameObject.SetActive(true);
    }

    public void HideInfoPanel()
    {
        infoGui.gameObject.SetActive(false);
    }

    public void ShowScorePanel()
    {
        scoreScreenGui.gameObject.SetActive(true);
        scoreScreen.UpdateScoreList();
    }

    public void HideScorePanel()
    {
        scoreScreenGui.gameObject.SetActive(false);
    }

    public void ChangePanel(RectTransform newPanel)
    {
        if (_currentPanel != null)
            _currentPanel.gameObject.SetActive(false);

        if (newPanel != null)
            newPanel.gameObject.SetActive(true);

        _currentPanel = newPanel;
    }

    public void ShowInGameMenu()
    {
        inGameMenu.gameObject.SetActive(true);
    }

    public void HideInGameMenu()
    {
        inGameMenu.gameObject.SetActive(false);
    }

    public void DisableAllPanels()
    {
        scoreScreenGui.gameObject.SetActive(false);
        lobbyGui.gameObject.SetActive(false);
        menuGui.gameObject.SetActive(false);
        infoGui.gameObject.SetActive(false);
        inGameMenu.gameObject.SetActive(false);
    }

    public void ToggleInGameMenu()
    {
        if (inGameMenu.gameObject.activeSelf)
        {
            HideInGameMenu();
        }
        else
        {
            ShowInGameMenu();
        }
    }

    public void StopClientCallback()
    {
        StopClient();
        StopMatchMaker();
        ChangePanel(menuGui);
        HideInfoPanel();
    }
}
