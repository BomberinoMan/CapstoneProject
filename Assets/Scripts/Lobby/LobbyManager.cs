﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.Networking.Match;

public class LobbyManager : NetworkLobbyManager
{
    public class PlayerInfo
    {
        public string name;
        public int connectionId;
        public bool isAlive;
        public int score;
    }

    public static LobbyManager _instance;

    public GameObject scoreScreenPlayerList;
    public RectTransform scoreScreenGui;
    public RectTransform lobbyGui;
    public RectTransform menuGui;
    public RectTransform countdownGui;
    public Text countdownText;
    public float countdownTime = 5.0f;
    public float scoreScreenTime = 5.0f;

    private List<NetworkLobbyPlayer> _players = new List<NetworkLobbyPlayer>();
    private List<PlayerInfo> connectedPlayerInfo = new List<PlayerInfo>();         //TODO: limit number of players
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


    // **************GAME**************

    //public void OnRoundFinished() { }

    public void Update()
    {
        //if all players but one dead
        //then return to lobby
    }

    public void OnGameFinished()
    {
        SendReturnToLobby();
    }

    public void PlayerDeath()
    {
        // if (all players dead) {
            //SendReturnToLobby();
        // }
    }

    // **************SERVER**************
    public override void OnLobbyStartServer()
    {
        base.OnLobbyStartServer();
        connectedPlayerInfo.Clear();
    }

    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        base.OnLobbyServerConnect(conn);

        if (conn.address != "localServer")
        {
            connectedPlayerInfo.Add( new PlayerInfo() { connectionId = conn.connectionId, isAlive = true, score = 0, name = "Anonymous" } );
        }
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        base.OnLobbyServerDisconnect(conn);
        connectedPlayerInfo.Remove(connectedPlayerInfo.Where(x => x.connectionId == conn.connectionId).FirstOrDefault());
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection networkConnection, short playerControllerId)
    {
        int i = getSlotIndex(networkConnection.connectionId);

        GameObject newPlayer = (GameObject)Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
        newPlayer.transform.position = _playerSpawnVectors[i];
        newPlayer.GetComponent<PlayerControllerComponent>().playerIndex = i;

        NetworkServer.Spawn(newPlayer);
        return newPlayer;
    }

    public void PlayerDead(PlayerControllerComponent player)
    {
        SpawnUpgradeInRandomLocation(UpgradeType.Bomb, player.maxNumBombs - 1);
        SpawnUpgradeInRandomLocation(UpgradeType.Laser, player.bombParams.radius - 2);
        SpawnUpgradeInRandomLocation(UpgradeType.Kick, player.bombKick);
        SpawnUpgradeInRandomLocation(UpgradeType.Line, player.bombLine);

        connectedPlayerInfo[player.playerIndex].isAlive = false;

        Invoke("CheckIfGameOver", 2);

        NetworkServer.Destroy(player.gameObject);
    }

    private void CheckIfGameOver()
    {
        if (connectedPlayerInfo.Where(x => x.isAlive).Count() == 1)
        {
            connectedPlayerInfo.Where(x => x.isAlive).First().score++;
            connectedPlayerInfo.Where(x => x.isAlive).First().isAlive = false;
        }

        if (connectedPlayerInfo.Where(x => x.isAlive).Count() == 0)
        {
            //  ServerReturnToLobby(); // This call doesn't work for some reason
            Debug.Log("GAME OVER!");
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        float remainingTime = scoreScreenTime;

        foreach (LobbyPlayer player in lobbySlots)
        {
            if (player != null)
            {
                connectedPlayerInfo.ForEach(x => player.RpcAddPlayerToScoreList(x.name, x.score));
            }
        }

        while (remainingTime >= -1)
        {
            yield return null;
            remainingTime -= Time.deltaTime;
        }

        //ServerChangeScene(lobbyScene);
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
    }

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

    private void SpawnUpgradeInRandomLocation(UpgradeType upgradeType, int num = 0)
    {
        for (int i = 0; i < num; i++)
        {
            Vector2 location = new Vector2();

            do
            {
                location.x = UnityEngine.Random.Range(1, 13);   // Spawnable locations on the board
                location.y = UnityEngine.Random.Range(1, 11);
            } while (Physics2D.RaycastAll(location, new Vector2(1.0f, 1.0f), 0.2f).Length != 0);

            switch (upgradeType)
            {
                case (UpgradeType.Bomb):
                    NetworkServer.Spawn(Instantiate(bombUpgrade, location, Quaternion.identity) as GameObject);
                    break;
                case (UpgradeType.Kick):
                    NetworkServer.Spawn(Instantiate(kickUpgrade, location, Quaternion.identity) as GameObject);
                    break;
                case (UpgradeType.Laser):
                    NetworkServer.Spawn(Instantiate(laserUpgrade, location, Quaternion.identity) as GameObject);
                    break;
                case (UpgradeType.Line):
                    NetworkServer.Spawn(Instantiate(lineUpgrade, location, Quaternion.identity) as GameObject);
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

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);

        Debug.Log("client error");
    }

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
        _currentPanel.gameObject.SetActive(false);
    }

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