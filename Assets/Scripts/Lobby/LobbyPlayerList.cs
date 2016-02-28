using UnityEngine;

public class LobbyPlayerList : MonoBehaviour
{
    public static LobbyPlayerList instance = null;
    public RectTransform playerListContentTransform;

    public void OnEnable()
    {
        instance = this;
    }

    public void AddPlayer(LobbyPlayer player)
    {
        player.transform.SetParent(playerListContentTransform, false);
    }
}
