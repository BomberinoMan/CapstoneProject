using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using System;
using System.Linq;

public class LobbyManager : NetworkLobbyManager
{
    public static LobbyManager _instance;

    public RectTransform lobbyGui;
    public RectTransform menuGui;
    public RectTransform countdownGui;
    public Text countdownText;
    public float countdownTime = 5.0f;

    private List<NetworkLobbyPlayer> _players = new List<NetworkLobbyPlayer>();
    private List<int> connectedPlayerIds = new List<int>();         //TODO: limit number of players
    private RectTransform _currentPanel;
    private Vector3[] _playerSpawnVectors = new Vector3[4]
    {
        new Vector3(1.0f, 11.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(12.0f, 1.0f, 0.0f),
        new Vector3(12.0f, 11.0f, 0.0f)
    };

    private bool sceneLoaded = false;

    public GameObject bombUpgrade;
    public GameObject laserUpgrade;
    public GameObject kickUpgrade;
    public GameObject lineUpgrade;
    public GameObject radioactiveUpgrade;

    public GameObject floor;
    public GameObject destructible;
    public GameObject indestructible;
    public GameObject[] playerAnimations;
    private BoardCreator boardCreator;

    void Start()
    {
        _instance = this;
        _currentPanel = menuGui;
    }


    // **************SERVER**************

    //public override void OnLobbyStartHost()
    //{
    //    base.OnLobbyStartHost();
    //    Debug.Log("OnLobbyStartHost()");
    //}

    //public override void OnLobbyStopHost()
    //{
    //    base.OnLobbyStopHost();
    //    Debug.Log("OnLobbyStopHost()");
    //}

    public override void OnLobbyStartServer()
    {
        base.OnLobbyStartServer();

        //Debug.Log("OnLobbyStartServer()");
        connectedPlayerIds.Clear();
    }

    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        base.OnLobbyServerConnect(conn);

        //Debug.Log("OnLobbyServerConnect(NetworkConnection networkConnection)");
        if (conn.address != "localServer")
        {
            connectedPlayerIds.Add(conn.connectionId);
        }
        
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        base.OnLobbyServerDisconnect(conn);

        //Debug.Log("OnLobbyServerDisconnect(NetworkConnection networkConnection)");
        connectedPlayerIds.Remove(conn.connectionId);
    }

    //public override void OnLobbyServerSceneChanged(string name)
    //{
    //    base.OnLobbyServerSceneChanged(name);_playerOrder
    //    Debug.Log("OnLobbyServerSceneChanged(string name)" + name);
    //}

    //public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection networkConnection, short other)
    //{
    //    Debug.Log("OnLobbyServerCreateLobbyPlayer(NetworkConnection networkConnection, short other)");
    //    return base.OnLobbyServerCreateLobbyPlayer(networkConnection, other);
    //}

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection networkConnection, short playerControllerId)
    {
        int i = getSlotIndex(networkConnection.connectionId);

        GameObject newPlayer = Instantiate(playerPrefab.gameObject);
        newPlayer.transform.position = _playerSpawnVectors[i];
        newPlayer.GetComponent<PlayerControllerComponent>().playerIndex = i;

        NetworkServer.Spawn(newPlayer);
        return newPlayer;
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject gameObject1, GameObject gameObject2)
    {
        if (!sceneLoaded)
            SpawnBoard();
        sceneLoaded = true;
        return true;
    }

    public override void OnLobbyServerPlayersReady()
    {
        foreach (LobbyPlayer player in lobbySlots)
        {
            if (player != null)
            {
                if (!player.readyToBegin)
                {
                    return;
                }
            }
        }

        StartCoroutine(CountDownCoroutine());
    }

    public IEnumerator CountDownCoroutine()
    {
        float remainingTime = countdownTime;
        int floorTime = Mathf.FloorToInt(remainingTime);

        while (remainingTime >= -1)
        {
            yield return null;

            remainingTime -= Time.deltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime);

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

        ServerChangeScene(playScene);
    }

    public void KickPlayer(NetworkConnection conn)
    {
        conn.Disconnect();
        //conn.Send(MsgType.RemovePlayer, new  RemovePlayerMessage());
    }

    //public void KickedMessageHandler(NetworkMessage msg)
    //{
    //    infoPanel.Display("Kicked by server", "Close", null);
    //    msg.conn.Disconnect();
    //}


    // **************CLIENT**************

    public override void OnLobbyClientEnter()
    {
        base.OnLobbyClientEnter();
        ChangePanel(lobbyGui);
    }

    private void SpawnBoard()
    {
        boardCreator = new BoardCreator();
        boardCreator.InitializeDestructible();

        //Initialize spawn for all connected players
        lobbySlots.Where(p => p != null).ToList()
            .ForEach(p => boardCreator.InitializeSpawn(_playerSpawnVectors[getSlotIndex(p.playerControllerId)]));

        //Initialize all upgrades
        boardCreator.InitializeUpgrades();
        //Get the generated tiles in the board
        var board = boardCreator.getBoard();

        //Spawn all objects in the board
        foreach (var tile in board.tiles)
        {
            if (tile.isIndestructible)
            {
                NetworkServer.Spawn(Instantiate(indestructible, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
                continue;
            }

            NetworkServer.Spawn(Instantiate(floor, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);

            if (tile.isDestructible)
                NetworkServer.Spawn(Instantiate(destructible, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);

            if (tile.isUpgrade)
                switch (tile.upgradeType)
                {
                    case (UpgradeType.Bomb):
                        NetworkServer.Spawn(Instantiate(bombUpgrade, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
                        break;
                    case (UpgradeType.Kick):
                        NetworkServer.Spawn(Instantiate(kickUpgrade, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
                        break;
                    case (UpgradeType.Laser):
                        NetworkServer.Spawn(Instantiate(laserUpgrade, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
                        break;
                    case (UpgradeType.Line):
                        NetworkServer.Spawn(Instantiate(lineUpgrade, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
                        break;
                    case (UpgradeType.Radioactive):
                        NetworkServer.Spawn(Instantiate(radioactiveUpgrade, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity) as GameObject);
                        break;
                    default: // Do nothing
                        break;
                }
        }
    }

    private int getSlotIndex(int connectionId)
    {
        int i = 0;
        foreach (var playerId in connectedPlayerIds)
        {
            if (playerId == connectionId)
                return i;
            i++;
        }

        Debug.LogError("No matching playerControllerId in slots");
        throw new ArgumentOutOfRangeException();
    }


    public override void OnLobbyClientExit()
    {
        base.OnLobbyClientExit();

        //Debug.Log("OnLobbyClientExit");
        ChangePanel(menuGui);
    }

    //public void OnRoundFinished()
    //{

    //}

    //public void OnGameFinished()
    //{
    //    ServerChangeScene(lobbyScene);
    //    ChangePanel(lobbyGui);
    //}

    //public override void OnLobbyClientConnect(NetworkConnection conn)
    //{
    //    base.OnLobbyClientConnect(conn);
    //    Debug.Log("OnLobbyClientConnect");
    //}

    //public override void OnLobbyClientDisconnect(NetworkConnection conn)
    //{
    //    base.OnLobbyClientDisconnect(conn);
    //    Debug.Log("OnLobbyClientDisconnect");
    //}

    //public override void OnLobbyStartClient(NetworkClient client)
    //{
    //    base.OnLobbyStartClient(client);
    //    Debug.Log("OnLobbyStartClient");
    //}

    //public override void OnLobbyStopClient()
    //{
    //    base.OnLobbyStopClient();
    //    Debug.Log("OnLobbyStopClient");
    //}

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        base.OnLobbyClientSceneChanged(conn);

        //Debug.Log("OnLobbyClientSceneChanged () = " + conn.connectionId);
        _currentPanel.gameObject.SetActive(false);
    }

    //public override void OnLobbyClientAddPlayerFailed()
    //{
    //    Debug.Log("OnLobbyClientAddPlayerFailed ()");
    //    base.OnLobbyClientAddPlayerFailed();
    //}

    //public override void OnStartHost()
    //{
    //    Debug.Log("OnStartHost");
    //    base.OnStartHost();
    //}


    // **************GUI**************

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


    // **************PLAYER LIST**************

    public void AddPlayer(LobbyPlayer player)
    {
        _players.Add(player);

    }

    public void RemovePlayer(LobbyPlayer player)
    {
        _players.Remove(player);
    }
}
