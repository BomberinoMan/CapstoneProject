using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LobbyManager : NetworkLobbyManager {
	public override void OnLobbyStartHost ()
	{
		Debug.Log ("OnLobbyStartHost()");
		base.OnLobbyStartHost ();
	}
}
