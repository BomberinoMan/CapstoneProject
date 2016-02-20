using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class LobbyManager : NetworkLobbyManager
{
    public static LobbyManager _instance;

    public RectTransform lobbyGui;
    public RectTransform menuGui;
    public RectTransform countdownGui;
    public Text countdownText;
    public GameObject[] playerAnimations;
    public float countdownTime = 5.0f;

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
        _playerOrder.Clear();
    }

    public override void OnLobbyServerConnect(NetworkConnection networkConnection)
    {
        base.OnLobbyServerConnect(networkConnection);

        //Debug.Log("OnLobbyServerConnect(NetworkConnection networkConnection)");
        if (networkConnection.address != "localServer")
            _playerOrder.Add(networkConnection.connectionId);
    }

    public override void OnLobbyServerDisconnect(NetworkConnection networkConnection)
    {
        base.OnLobbyServerDisconnect(networkConnection);

        //Debug.Log("OnLobbyServerDisconnect(NetworkConnection networkConnection)");
        _playerOrder.Remove(networkConnection.connectionId);
    }

    //public override void OnLobbyServerSceneChanged(string name)
    //{
    //    base.OnLobbyServerSceneChanged(name);
    //    Debug.Log("OnLobbyServerSceneChanged(string name)" + name);
    //}

    //public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection networkConnection, short other)
    //{
    //    Debug.Log("OnLobbyServerCreateLobbyPlayer(NetworkConnection networkConnection, short other)");
    //    return base.OnLobbyServerCreateLobbyPlayer(networkConnection, other);
    //}

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection networkConnection, short other)
    {
        //Debug.Log("OnLobbyServerCreateGamePlayer(NetworkConnection networkConnection, short other)");

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

    //public override void OnLobbyServerPlayerRemoved(NetworkConnection networkConnection, short other)
    //{
    //    Debug.Log("OnLobbyServerPlayerRemoved(NetworkConnection networkConnection, short other)");
    //    base.OnLobbyServerPlayerRemoved(networkConnection, other);
    //}

    //public override bool OnLobbyServerSceneLoadedForPlayer(GameObject gameObject1, GameObject gameObject2)
    //{
    //    Debug.Log("OnLobbyServerSceneLoadedForPlayer(GameObject gameObject1, GameObject gameObject2)");
    //    return base.OnLobbyServerSceneLoadedForPlayer(gameObject1, gameObject2);
    //}

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
        //ServerChangeScene(playScene);
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

        //Debug.Log("OnLobbyClientEnter");
        ChangePanel(lobbyGui);
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
