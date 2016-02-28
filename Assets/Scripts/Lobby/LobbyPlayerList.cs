using UnityEngine;

public class LobbyPlayerList : MonoBehaviour
{
    public static LobbyPlayerList _instance = null;
    public RectTransform playerListContentTransform;

    public void OnEnable()
    {
        _instance = this;
    }

    public void AddPlayer(LobbyPlayer player)
    {
        player.transform.SetParent(playerListContentTransform, false);
    }
}
