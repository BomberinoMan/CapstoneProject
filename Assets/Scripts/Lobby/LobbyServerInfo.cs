using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

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
        joinButton.onClick.AddListener(() => { JoinMatchCallback(match.networkId); });
    }

    public void JoinMatchCallback(NetworkID networkId)
    {
        Debug.Log("NETWORKID: " + networkId);
        LobbyManager.instance.matchMaker.JoinMatch(networkId, "", LobbyManager.instance.OnMatchJoined);
    }

}
