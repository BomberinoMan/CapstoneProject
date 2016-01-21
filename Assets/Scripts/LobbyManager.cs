using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LobbyManager : NetworkLobbyManager {
	public override void OnLobbyStartHost ()
	{
		Debug.Log ("OnLobbyStartHost()");
		base.OnLobbyStartHost ();
	}

	public override void OnLobbyStopHost ()
	{
		Debug.Log ("OnLobbyStopHost()");
		base.OnLobbyStopHost ();
	}

	public override void OnLobbyStartServer ()
	{
		Debug.Log ("OnLobbyStartServer()");
		base.OnLobbyStartServer ();
	}

	public override void OnLobbyServerConnect (NetworkConnection networkConnection)
	{
		Debug.Log ("OnLobbyServerConnect(NetworkConnection networkConnection)");
		base.OnLobbyServerConnect (networkConnection);
	}

	public override void OnLobbyServerDisconnect (NetworkConnection networkConnection)
	{
		Debug.Log ("OnLobbyServerDisconnect(NetworkConnection networkConnection)");
		base.OnLobbyServerDisconnect (networkConnection);
	}

	public override void OnLobbyServerSceneChanged (string name)
	{
		Debug.Log ("OnLobbyServerSceneChanged(string name)");
		base.OnLobbyServerSceneChanged (name);
	}

	public override GameObject OnLobbyServerCreateLobbyPlayer (NetworkConnection networkConnection, short other)
	{
		Debug.Log ("OnLobbyServerCreateLobbyPlayer(NetworkConnection networkConnection, short other)");
		return base.OnLobbyServerCreateLobbyPlayer (networkConnection, other);
	}

	public override GameObject OnLobbyServerCreateGamePlayer (NetworkConnection networkConnection, short other)
	{
		Debug.Log ("OnLobbyServerCreateGamePlayer(NetworkConnection networkConnection, short other)");
		return base.OnLobbyServerCreateGamePlayer (networkConnection, other);
	}

	public override void OnLobbyServerPlayerRemoved (NetworkConnection networkConnection, short other)
	{
		Debug.Log ("OnLobbyServerPlayerRemoved(NetworkConnection networkConnection, short other)");
		base.OnLobbyServerPlayerRemoved (networkConnection, other);
	}

	public override bool OnLobbyServerSceneLoadedForPlayer (GameObject gameObject1, GameObject gameObject2)
	{
		Debug.Log ("OnLobbyServerSceneLoadedForPlayer(GameObject gameObject1, GameObject gameObject2)");
		return base.OnLobbyServerSceneLoadedForPlayer (gameObject1, gameObject2);
	}

	public override void OnLobbyServerPlayersReady ()
	{
		Debug.Log ("OnLobbyServerPlayersReady()");
		base.OnLobbyServerPlayersReady ();
	}
}
