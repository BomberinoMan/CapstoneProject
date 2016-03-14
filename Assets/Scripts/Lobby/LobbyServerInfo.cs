using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System;

public class LobbyServerInfo : MonoBehaviour
{
    public Text serverInfo;
    public Text slotInfo;
    public Button joinButton;

    public void PopulateMatchInfo(MatchDesc match)
    {
        serverInfo.text = match.name;
        slotInfo.text = match.currentSize.ToString() + "/" + match.maxSize.ToString();

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() => { OnClickJoin(match.networkId, match.hostNodeId); });
    }

    public void OnClickJoin(NetworkID networkId, NodeID nodeId)
    {
        LobbyManager.instance.DisplayInfoNotification("Joining...");
        LobbyManager.instance.matchMaker.JoinMatch(networkId, "", LobbyManager.instance.OnMatchJoined);
    }
}
