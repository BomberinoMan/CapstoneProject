using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class LobbyManager : NetworkLobbyManager
{
    public static LobbyManager _instance;

    public RectTransform lobbyGui;
    public RectTransform menuGui;
    public GameObject[] playerAnimations;

    private List<NetworkLobbyPlayer> _players = new List<NetworkLobbyPlayer>();
    private RectTransform _currentPanel;
    private IList<int> _playerOrder = new List<int>(); //TODO LIMIT ON THE NUMBER OF PLAYERS
    private Vector3[] _playerSpawnVectors = new Vector3[4]
    {   new Vector3(0.0f, 10.0f, 0.0f),
        new Vector3(12.0f, 0.0f, 0.0f),
        new Vector3(12.0f, 0.0f, 0.0f),
        new Vector3(12.0f, 10.0f, 0.0f)
    };

    void Start()
    {
        _instance = this;
        _currentPanel = menuGui;
        //DontDestroyOnLoad(gameObject)     //already enable on unity script component
    }

    // **************SERVER**************
    public override void OnLobbyStartHost()
    {
        Debug.Log("OnLobbyStartHost()");
        base.OnLobbyStartHost();
    }

    public override void OnLobbyStopHost()
    {
        Debug.Log("OnLobbyStopHost()");
        base.OnLobbyStopHost();
    }

    public override void OnLobbyStartServer()
    {
        Debug.Log("OnLobbyStartServer()");

        _playerOrder.Clear();

        base.OnLobbyStartServer();
    }

    public override void OnLobbyServerConnect(NetworkConnection networkConnection)
    {
        Debug.Log("OnLobbyServerConnect(NetworkConnection networkConnection)");
        if (networkConnection.address != "localServer")
            _playerOrder.Add(networkConnection.connectionId);
        base.OnLobbyServerConnect(networkConnection);
    }

    public override void OnLobbyServerDisconnect(NetworkConnection networkConnection)
    {
        Debug.Log("OnLobbyServerDisconnect(NetworkConnection networkConnection)");

        _playerOrder.Remove(networkConnection.connectionId);

        base.OnLobbyServerDisconnect(networkConnection);
    }

    public override void OnLobbyServerSceneChanged(string name)
    {
        Debug.Log("OnLobbyServerSceneChanged(string name)");
        base.OnLobbyServerSceneChanged(name);
    }

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection networkConnection, short other)
    {

        Debug.Log("OnLobbyServerCreateLobbyPlayer(NetworkConnection networkConnection, short other)");
        return base.OnLobbyServerCreateLobbyPlayer(networkConnection, other);
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection networkConnection, short other)
    {
        Debug.Log("OnLobbyServerCreateGamePlayer(NetworkConnection networkConnection, short other)");

        int i = 0;
        GameObject newPlayer = null;
        GameObject playerAnimation;

        foreach (var id in _playerOrder)
        {
            if (id == networkConnection.connectionId)
                break;
            i++;
        }

        newPlayer = Instantiate(playerPrefab.gameObject);

        playerAnimation = Instantiate(playerAnimations[i]);
        playerAnimation.transform.SetParent(newPlayer.transform);
        newPlayer.transform.position = _playerSpawnVectors[i];

        newPlayer.GetComponent<PlayerControllerComponent>().playerNum = i;

        NetworkServer.Spawn(newPlayer);

        return newPlayer;
    }

    public override void OnLobbyServerPlayerRemoved(NetworkConnection networkConnection, short other)
    {
        Debug.Log("OnLobbyServerPlayerRemoved(NetworkConnection networkConnection, short other)");
        base.OnLobbyServerPlayerRemoved(networkConnection, other);
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject gameObject1, GameObject gameObject2)
    {
        _currentPanel.gameObject.SetActive(false);
        Debug.Log("OnLobbyServerSceneLoadedForPlayer(GameObject gameObject1, GameObject gameObject2)");
        return base.OnLobbyServerSceneLoadedForPlayer(gameObject1, gameObject2);

    }

    public override void OnLobbyServerPlayersReady()
    {
        Debug.Log("OnLobbyServerPlayersReady()");
        base.OnLobbyServerPlayersReady();
    }


    // **************CLIENT**************

    public override void OnLobbyClientEnter()
    {
        Debug.Log("OnLobbyClientEnter ()");

        base.OnLobbyClientEnter();
    }

    public override void OnLobbyClientExit()
    {
        Debug.Log("OnLobbyClientExit ()");
        base.OnLobbyClientExit();
    }

    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnLobbyClientConnect ()");
        base.OnLobbyClientConnect(conn);
    }

    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnLobbyClientDisconnect ()");
        base.OnLobbyClientDisconnect(conn);
    }

    public override void OnLobbyStartClient(NetworkClient client)
    {
        Debug.Log("OnLobbyStartClient ()");
        base.OnLobbyStartClient(client);
    }

    public override void OnLobbyStopClient()
    {
        Debug.Log("OnLobbyStopClient ()");
        base.OnLobbyStopClient();
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log("OnLobbyClientSceneChanged ()");
        base.OnLobbyClientSceneChanged(conn);
    }

    public override void OnLobbyClientAddPlayerFailed()
    {
        Debug.Log("OnLobbyClientAddPlayerFailed ()");
        base.OnLobbyClientAddPlayerFailed();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        ChangePanel(lobbyGui);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ChangePanel(lobbyGui);
        //if (!NetworkServer.active)
        //{
        //    ChangePanel(lobbyGui);
        //}
    }

    public void ChangePanel(RectTransform newPanel)
    {
        if (_currentPanel != null)
        {
            _currentPanel.gameObject.SetActive(false);
        }

        if (newPanel != null)
        {
            newPanel.gameObject.SetActive(true);
        }

        _currentPanel = newPanel;
    }

    public void AddPlayer(LobbyPlayer player)
    {
        _players.Add(player);

    }

    public void RemovePlayer(LobbyPlayer player)
    {
        _players.Remove(player);
    }
}
