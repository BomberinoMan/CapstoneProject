using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System;

public class LobbyServerInfo : MonoBehaviour
{
	private string _ip;
    public Text serverName;
    public Button joinButton;

	public void PopulateMatchInfo(string name, string ip, bool enableButton = true)
    {
		_ip = ip;
		serverName.text = name;
        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() => { OnClickJoin(); });
		joinButton.gameObject.SetActive (enableButton);
    }

    public void OnClickJoin()
    {
		LobbyManager.instance.DisplayInfoNotification("Joining" + _ip);
		LobbyManager.instance.networkAddress = _ip;
		LobbyManager.instance.StartClient ();
    }
}
