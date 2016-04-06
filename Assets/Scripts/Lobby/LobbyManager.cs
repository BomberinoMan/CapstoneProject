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
using System.Threading;

public class LobbyManager : NetworkLobbyManager
{
    public GameObject gameManager;
    public static LobbyManager instance;
    public ScoreScreen scoreScreen;

    public RectTransform scoreScreenGui;
    public RectTransform lobbyGui;
    public RectTransform menuGui;
    public RectTransform infoGui;
    public RectTransform inGameMenu;

    public Text infoText;
	public InputField infoInputField;
    public Button infoButton;
    public float countdownTime = 5.0f;

    public bool isMatchMaking = false;
	public string roomName = "";
    public LobbyPlayer localPlayer;

    private RectTransform _currentPanel;
    private bool _sceneLoaded = false;
    private GameObject[] playerGameObjects = new GameObject[4];

    private bool _isResponse;
    private DeleteRoomResponse _response;
    private Action<DeleteRoomResponse> _responseCallback;

    void Start()
    {
        instance = this;

        _isResponse = false;
        _responseCallback = null;

        // Activate needed objects to play the game
        gameManager.SetActive(true);
        ChangePanel(menuGui);
    }

    void Update()
    {
        if (_isResponse)
        {
            if (_responseCallback != null)
            {
                _responseCallback(_response);
            }
            _isResponse = false;
        }
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

        _isResponse = false;
        _responseCallback = new Action<DeleteRoomResponse>(DeleteRoomCallback);
        new Thread(DeleteRoom).Start();
    }

    public void DeleteRoom()
    {
		_response = DBConnection.instance.DeleteRoom (new DeleteRoomMessage { userId = LoginInformation.guid });
        _isResponse = true;
    }

    public void DeleteRoomCallback(DeleteRoomResponse response)
    {
        //Do Nothing
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
				lobbySlots.Where (x => x != null).ToList ().ForEach (x => (x as LobbyPlayer).RpcCancelCountdown ());
                yield break;
            }

            if (newFloorTime != floorTime)
            {
                floorTime = newFloorTime;
				lobbySlots.Where (x => x != null).ToList ().ForEach (x => (x as LobbyPlayer).RpcUpdateCountdown(floorTime));
            }
        }

        for (int i = 0; i < lobbySlots.Count(); i++)
            if (lobbySlots[i] != null)
                (lobbySlots[i] as LobbyPlayer).isAlive = true;

        ServerChangeScene(playScene);
    }

    public bool ArePlayersReady()
    {
        foreach (LobbyPlayer player in lobbySlots)
			if (player != null)
				if (!player.readyToBegin)
                    return false;
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
		LobbyRoom.instance.SetRoomName (roomName);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.LogError("CLIENT ERROR" + errorCode);
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == "Game")
            DisableAllPanels();
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

	public void DisplayInfoInputField(string placeholder, Action<string> onSubmit){
		infoText.gameObject.SetActive (false);

		infoInputField.gameObject.SetActive (true);
        infoButton.gameObject.SetActive(true);
		infoInputField.GetComponentInChildren<Text> ().text = placeholder;

		infoButton.GetComponentInChildren<Text> ().text = "Submit";
		infoButton.onClick.RemoveAllListeners ();
		infoButton.onClick.AddListener (delegate {
			ResetInfoPanel();
			onSubmit(infoInputField.text);
		});

		infoGui.gameObject.SetActive (true);
	}

	public void ResetInfoPanel(){
		infoInputField.gameObject.SetActive (false);
		infoButton.GetComponentInChildren<Text> ().text = "Cancel";
        infoButton.gameObject.SetActive(true);
		infoText.gameObject.SetActive (true);
		infoGui.gameObject.SetActive (false);
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
